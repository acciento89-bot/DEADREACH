using System;
using Kamilunavo.Deadreach.Core;
using Kamilunavo.Deadreach.Persistence;
using Kamilunavo.Deadreach.Progression;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamilunavo.Deadreach.UI
{
    /// <summary>
    /// Large release-hardening pass for Production 0.14.
    /// Keeps the accepted command-center art/layout and hardens the remaining MVP path:
    /// safe-area handling, mobile touch targets, deployment locking, persistence on app state
    /// changes, live profile refresh, action feedback, and Arsenal salvage actions.
    /// </summary>
    public sealed partial class Production14CommandCenterUI
    {
        private Rect _releaseLastSafeArea = new(-1f, -1f, -1f, -1f);
        private Vector2Int _releaseLastScreenSize = new(-1, -1);
        private Button _releaseDeployButton;
        private bool _releaseDeployWired;
        private bool _releaseDeploying;

        private RectTransform _releaseToastRoot;
        private Text _releaseToastText;
        private CanvasGroup _releaseToastGroup;
        private float _releaseToastUntil;

        private bool _releaseSnapshotReady;
        private int _releaseScrap;
        private int _releaseSelectedLevel;
        private int _releaseStashCount;
        private int _releaseEquippedUpgradeLevel;
        private int _releaseWorkbench;
        private int _releaseMedbay;
        private int _releaseCargoRig;
        private int _releaseScavenger;
        private string _releaseCharacterId = string.Empty;
        private string _releaseWeaponId = string.Empty;

        private void Update()
        {
            if (_root == null)
                return;

            ApplyReleaseSafeAreaIfNeeded();
            WireReleaseDeployButton();
            EnsureReleaseActionToast();
            EnsureReleaseArsenalSalvageActions();
            TrackReleaseProfileChanges();
            TickReleaseActionToast();
        }

        private void ApplyReleaseSafeAreaIfNeeded()
        {
            if (Screen.width <= 0 || Screen.height <= 0)
                return;

            var safe = Screen.safeArea;
            var size = new Vector2Int(Screen.width, Screen.height);
            if (safe == _releaseLastSafeArea && size == _releaseLastScreenSize)
                return;

            _releaseLastSafeArea = safe;
            _releaseLastScreenSize = size;

            // Production14CommandCenterUI owns the single responsive frame/scaler implementation.
            // Do not re-apply the old 1600x900 scaler here; it would undo the final readability pass.
            ApplyResponsiveFrame(true);

            if (EventSystem.current != null)
                EventSystem.current.pixelDragThreshold = Mathf.Max(EventSystem.current.pixelDragThreshold, 12);

            EnforceReleaseTouchTargets();
        }

        private void EnforceReleaseTouchTargets()
        {
            if (_root == null)
                return;

            foreach (var button in _root.GetComponentsInChildren<Button>(true))
            {
                if (button == null)
                    continue;

                var layout = button.GetComponent<LayoutElement>();
                if (layout == null)
                    layout = button.gameObject.AddComponent<LayoutElement>();

                layout.minHeight = Mathf.Max(layout.minHeight, 48f);
                layout.minWidth = Mathf.Max(layout.minWidth, 92f);
            }

            foreach (var scroll in _root.GetComponentsInChildren<ScrollRect>(true))
            {
                if (scroll == null)
                    continue;

                scroll.inertia = true;
                scroll.decelerationRate = 0.12f;
                scroll.scrollSensitivity = Mathf.Max(scroll.scrollSensitivity, 28f);
                scroll.movementType = ScrollRect.MovementType.Clamped;
            }
        }

        private void WireReleaseDeployButton()
        {
            if (_releaseDeployWired || _root == null)
                return;

            var deploy = _root.Find("Footer/Deploy")?.GetComponent<Button>();
            if (deploy == null)
                return;

            deploy.onClick.RemoveAllListeners();
            deploy.onClick.AddListener(TryReleaseDeploy);
            _releaseDeployButton = deploy;
            _releaseDeployWired = true;
        }

        private void TryReleaseDeploy()
        {
            if (_releaseDeploying)
                return;

            var data = SaveService.Data;
            if (data.selectedLevel < 1 || data.selectedLevel > Mathf.Clamp(data.highestUnlockedLevel, 1, SaveService.MaxCampaignLevel))
            {
                ShowReleaseAction("DEPLOY BLOCKED // INVALID CAMPAIGN LEVEL", _danger, 2.6f);
                return;
            }

            if (string.IsNullOrWhiteSpace(data.selectedCharacterId) ||
                data.unlockedCharacterIds == null ||
                !data.unlockedCharacterIds.Contains(data.selectedCharacterId))
            {
                ShowReleaseAction("DEPLOY BLOCKED // SELECT AN OPERATOR", _danger, 2.6f);
                return;
            }

            SaveService.Save();
            _releaseDeploying = true;
            if (_releaseDeployButton != null)
                _releaseDeployButton.interactable = false;

            ShowReleaseAction(
                $"DEPLOYMENT INITIALIZED // LEVEL {data.selectedLevel:00} // {RunDifficultyDirector.GetZoneName(data.selectedLevel).ToUpperInvariant()}",
                _green,
                2.0f);

            if (SceneFlowService.LoadExpedition())
                return;

            _releaseDeploying = false;
            if (_releaseDeployButton != null)
                _releaseDeployButton.interactable = true;
            ShowReleaseAction("DEPLOY FAILED // EXPEDITION SCENE NOT AVAILABLE", _danger, 3.2f);
        }

        private void EnsureReleaseActionToast()
        {
            if (_releaseToastRoot != null || _root == null)
                return;

            _releaseToastRoot = CreateIndustrialPanel(
                "ReleaseActionToast",
                _root,
                Production14IndustrialSkin.PlateKind.Tag,
                false);
            Place(_releaseToastRoot, 0.335f, 0.102f, 0.665f, 0.151f);
            _releaseToastRoot.SetAsLastSibling();

            _releaseToastGroup = _releaseToastRoot.gameObject.AddComponent<CanvasGroup>();
            _releaseToastGroup.alpha = 0f;
            _releaseToastGroup.interactable = false;
            _releaseToastGroup.blocksRaycasts = false;

            _releaseToastText = CreateLabel(
                "Text",
                _releaseToastRoot,
                string.Empty,
                9,
                FontStyle.Bold,
                _cyan,
                TextAnchor.MiddleCenter);
            Fill(_releaseToastText.rectTransform, 12f, 3f, 12f, 3f);
        }

        private void ShowReleaseAction(string message, Color accent, float seconds = 2.2f)
        {
            EnsureReleaseActionToast();
            if (_releaseToastText == null || _releaseToastGroup == null)
                return;

            _releaseToastText.text = message;
            _releaseToastText.color = accent;
            _releaseToastGroup.alpha = 1f;
            _releaseToastUntil = Time.unscaledTime + Mathf.Max(0.5f, seconds);
            _releaseToastRoot.SetAsLastSibling();
        }

        private void TickReleaseActionToast()
        {
            if (_releaseToastGroup == null || _releaseToastGroup.alpha <= 0f)
                return;

            if (Time.unscaledTime <= _releaseToastUntil)
                return;

            _releaseToastGroup.alpha = Mathf.MoveTowards(
                _releaseToastGroup.alpha,
                0f,
                Time.unscaledDeltaTime * 5.5f);
        }

        private void EnsureReleaseArsenalSalvageActions()
        {
            if (_activeNavIndex != 1 || _contentRoot == null)
                return;

            var content = _contentRoot.Find("ArsenalInventory/InventoryScrollHost/Viewport/ScrollContent");
            var data = SaveService.Data;
            if (content == null || data.stashWeapons == null)
                return;

            for (var i = 0; i < data.stashWeapons.Count; i++)
            {
                var weapon = data.stashWeapons[i];
                if (weapon == null || string.Equals(weapon.instanceId, data.equippedPrimaryWeaponId, StringComparison.Ordinal))
                    continue;

                var card = content.Find($"Weapon_{i:00}") as RectTransform;
                if (card == null || card.Find("ReleaseSalvage") != null)
                    continue;

                var equipRect = card.Find("Equip") as RectTransform;
                if (equipRect != null)
                    Place(equipRect, 0.858f, 0.20f, 0.968f, 0.80f);

                var capturedId = weapon.instanceId;
                var value = SaveService.GetSalvageValue(weapon);
                var salvage = CreateScreenButton(
                    "ReleaseSalvage",
                    card,
                    $"SALVAGE\n+{value:N0}",
                    _amber,
                    () =>
                    {
                        if (!SaveService.SalvageWeapon(capturedId))
                            return;

                        RefreshHeaderCounters();
                        RefreshFooter();
                        CaptureReleaseSnapshot();
                        ShowReleaseAction($"SALVAGE COMPLETE // +{value:N0} SCRAP", _amber, 2.2f);
                        HandleNav(1);
                    });
                Place(salvage.GetComponent<RectTransform>(), 0.725f, 0.20f, 0.848f, 0.80f);
            }
        }

        private void TrackReleaseProfileChanges()
        {
            if (!_releaseSnapshotReady)
            {
                CaptureReleaseSnapshot();
                return;
            }

            var data = SaveService.Data;
            var currentWeapon = SaveService.GetEquippedPrimaryWeapon();
            var currentWeaponId = data.equippedPrimaryWeaponId ?? string.Empty;
            var currentUpgrade = currentWeapon?.upgradeLevel ?? -1;
            var stashCount = data.stashWeapons?.Count ?? 0;

            string message = null;
            var accent = _cyan;

            if (!string.Equals(_releaseCharacterId, data.selectedCharacterId ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                var op = OperatorCatalog.Get(data.selectedCharacterId);
                message = $"OPERATOR {op.Name.ToUpperInvariant()} // ACTIVE";
                accent = op.Accent;
            }
            else if (_releaseSelectedLevel != data.selectedLevel)
            {
                message = $"CAMPAIGN TARGET // LEVEL {data.selectedLevel:00} SELECTED";
                accent = _green;
            }
            else if (!string.Equals(_releaseWeaponId, currentWeaponId, StringComparison.Ordinal))
            {
                message = currentWeapon != null
                    ? $"PRIMARY EQUIPPED // {currentWeapon.displayNameSnapshot.ToUpperInvariant()} // POWER {currentWeapon.itemPower:000}"
                    : "PRIMARY LOADOUT // FIELD ISSUE ACTIVE";
                accent = _green;
            }
            else if (_releaseEquippedUpgradeLevel != currentUpgrade && currentWeapon != null)
            {
                message = $"WEAPON CALIBRATED // {currentWeapon.displayNameSnapshot.ToUpperInvariant()} // CAL {currentWeapon.upgradeLevel:00}";
                accent = _green;
            }
            else if (_releaseWorkbench != data.workbenchLevel ||
                     _releaseMedbay != data.medbayLevel ||
                     _releaseCargoRig != data.cargoRigLevel ||
                     _releaseScavenger != data.scavengerNetworkLevel)
            {
                message = "BUNKER SYSTEM UPGRADED // PROFILE SAVED";
                accent = _green;
            }
            else if (_releaseStashCount != stashCount)
            {
                message = $"ARSENAL UPDATED // {stashCount:00} SECURED WEAPONS";
                accent = _cyan;
            }
            else if (_releaseScrap != data.securedScrap)
            {
                var delta = data.securedScrap - _releaseScrap;
                message = delta >= 0
                    ? $"SCRAP SECURED // +{delta:N0}"
                    : $"SCRAP SPENT // {Mathf.Abs(delta):N0}";
                accent = delta >= 0 ? _green : _amber;
            }

            if (message != null)
            {
                RefreshHeaderCounters();
                RefreshFooter();
                ShowReleaseAction(message, accent, 2.2f);
            }

            CaptureReleaseSnapshot();
        }

        private void CaptureReleaseSnapshot()
        {
            var data = SaveService.Data;
            var equipped = SaveService.GetEquippedPrimaryWeapon();

            _releaseScrap = data.securedScrap;
            _releaseSelectedLevel = data.selectedLevel;
            _releaseStashCount = data.stashWeapons?.Count ?? 0;
            _releaseCharacterId = data.selectedCharacterId ?? string.Empty;
            _releaseWeaponId = data.equippedPrimaryWeaponId ?? string.Empty;
            _releaseEquippedUpgradeLevel = equipped?.upgradeLevel ?? -1;
            _releaseWorkbench = data.workbenchLevel;
            _releaseMedbay = data.medbayLevel;
            _releaseCargoRig = data.cargoRigLevel;
            _releaseScavenger = data.scavengerNetworkLevel;
            _releaseSnapshotReady = true;
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused)
                SaveService.Save();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused)
                SaveService.Save();
        }

        private void OnApplicationQuit()
        {
            SaveService.Save();
        }
    }
}
