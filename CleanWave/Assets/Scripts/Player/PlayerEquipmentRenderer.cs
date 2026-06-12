using UnityEngine;

namespace CleanWave
{
    /// <summary>
    /// Manages tongs and bag overlay sprites on the player.
    /// Attach to the Player root. Requires child GameObjects named
    /// "TongsOverlay" and "BagOverlay" each with a SpriteRenderer.
    /// </summary>
    public class PlayerEquipmentRenderer : MonoBehaviour
    {
        [Header("Tongs sprites (Lv1 / Lv2 / Lv3)")]
        [SerializeField] private Sprite[] tongsSprites = new Sprite[3];

        [Header("Bag sprites (Lv1 / Lv2 / Lv3)")]
        [SerializeField] private Sprite[] bagSprites = new Sprite[3];

        [Header("Overlay renderers (child objects)")]
        [SerializeField] private SpriteRenderer tongsRenderer;
        [SerializeField] private SpriteRenderer bagRenderer;

        [Header("Position offsets per direction")]
        [SerializeField] private Vector2 tongsOffsetDown  = new Vector2( 0.5f, -0.6f);
        [SerializeField] private Vector2 tongsOffsetUp    = new Vector2( 0.5f, -0.3f);
        [SerializeField] private Vector2 tongsOffsetSide  = new Vector2( 0.7f, -0.4f);
        [SerializeField] private Vector2 bagOffsetDefault = new Vector2(-0.3f,  0.1f);

        private int tongsLevel = 0;
        private int bagLevel   = 0;
        private PlayerController playerController;
        private SpriteRenderer baseRenderer;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            baseRenderer     = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (playerController == null) return;

            string dir  = playerController.CurrentDirection;
            bool facingLeft = baseRenderer != null && baseRenderer.flipX;

            UpdateTongsPosition(dir, facingLeft);
            UpdateBagPosition(facingLeft);

            // Mirror overlays with player
            if (tongsRenderer != null) tongsRenderer.flipX = facingLeft;
            if (bagRenderer   != null) bagRenderer.flipX   = facingLeft;
        }

        private void UpdateTongsPosition(string dir, bool facingLeft)
        {
            if (tongsRenderer == null) return;

            Vector2 offset = dir switch
            {
                "up"   => tongsOffsetUp,
                "side" => tongsOffsetSide,
                _      => tongsOffsetDown,
            };

            if (facingLeft) offset.x = -offset.x;
            tongsRenderer.transform.localPosition = offset;

            // Hide tongs on back-view (up) — tongs would be in front anyway
            tongsRenderer.enabled = true;
        }

        private void UpdateBagPosition(bool facingLeft)
        {
            if (bagRenderer == null) return;
            Vector2 offset = bagOffsetDefault;
            if (facingLeft) offset.x = -offset.x;
            bagRenderer.transform.localPosition = offset;
        }

        // ── Called by UpgradeShop when upgrade is applied ──

        public void SetTongsLevel(int level)
        {
            tongsLevel = Mathf.Clamp(level, 0, 2);
            if (tongsRenderer != null && tongsSprites != null && tongsSprites.Length > tongsLevel)
                tongsRenderer.sprite = tongsSprites[tongsLevel];
        }

        public void SetBagLevel(int level)
        {
            bagLevel = Mathf.Clamp(level, 0, 2);
            if (bagRenderer != null && bagSprites != null && bagSprites.Length > bagLevel)
                bagRenderer.sprite = bagSprites[bagLevel];
        }

        public int TongsLevel => tongsLevel;
        public int BagLevel   => bagLevel;
    }
}
