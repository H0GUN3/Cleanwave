using CleanWave.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CleanWave.UI
{
    public class CoinHudView : MonoBehaviour
    {
        private const string DefaultDigitPathPrefix = "MiryeongUI/num_";

        [SerializeField] private Transform digitRoot;
        [SerializeField] private float digitHeight = 28f;
        [SerializeField] private float digitSpacing = 1f;
        [SerializeField] private string digitResourcePathPrefix = DefaultDigitPathPrefix;

        private readonly List<Image> digitImages = new List<Image>();
        private readonly Sprite[] digitSprites = new Sprite[10];

        private CoinWallet wallet;

        public void Initialize(Transform digitsParent, Sprite[] sprites, float targetDigitHeight, float spacing)
        {
            digitRoot = digitsParent;
            digitHeight = Mathf.Max(1f, targetDigitHeight);
            digitSpacing = Mathf.Max(0f, spacing);

            if (sprites == null)
                return;

            int count = Mathf.Min(digitSprites.Length, sprites.Length);
            for (int i = 0; i < count; i++)
                digitSprites[i] = sprites[i];

            LoadMissingDigitSprites();
        }

        private void Awake()
        {
            LoadMissingDigitSprites();
        }

        private void Start()
        {
            Bind(CoinWallet.EnsureInstance());
        }

        private void OnDestroy()
        {
            if (wallet != null)
                wallet.CoinsChanged -= SetCoins;
        }

        public void Bind(CoinWallet coinWallet)
        {
            if (wallet != null)
                wallet.CoinsChanged -= SetCoins;

            wallet = coinWallet;
            if (wallet == null)
            {
                SetCoins(0);
                return;
            }

            wallet.CoinsChanged += SetCoins;
            SetCoins(wallet.Coins);
        }

        public void SetCoins(int coins)
        {
            if (digitRoot == null)
                return;

            LoadMissingDigitSprites();

            string coinString = Mathf.Max(0, coins).ToString();
            EnsureDigitImages(coinString.Length);

            for (int i = 0; i < digitImages.Count; i++)
            {
                bool isVisible = i < coinString.Length;
                Image image = digitImages[i];
                image.gameObject.SetActive(isVisible);

                if (!isVisible)
                    continue;

                int digit = coinString[i] - '0';
                image.sprite = digitSprites[digit];
                image.preserveAspect = true;
                SetDigitSize(image.rectTransform, image.sprite);
            }
        }

        private void EnsureDigitImages(int neededCount)
        {
            CacheExistingDigitImages();

            while (digitImages.Count < neededCount)
            {
                GameObject digitObject = new GameObject($"CoinDigit_{digitImages.Count}");
                digitObject.transform.SetParent(digitRoot, false);

                Image image = digitObject.AddComponent<Image>();
                image.raycastTarget = false;
                image.preserveAspect = true;
                digitImages.Add(image);
            }
        }

        private void CacheExistingDigitImages()
        {
            if (digitImages.Count > 0 || digitRoot == null)
                return;

            Image[] existingImages = digitRoot.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < existingImages.Length; i++)
                digitImages.Add(existingImages[i]);
        }

        private void LoadMissingDigitSprites()
        {
            string pathPrefix = string.IsNullOrWhiteSpace(digitResourcePathPrefix)
                ? DefaultDigitPathPrefix
                : digitResourcePathPrefix;

            for (int i = 0; i < digitSprites.Length; i++)
            {
                if (digitSprites[i] != null)
                    continue;

                digitSprites[i] = LoadSprite(pathPrefix + i);
            }
        }

        private static Sprite LoadSprite(string resourcePath)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);
            if (sprites != null && sprites.Length > 0)
                return sprites[0];

            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite != null)
                return sprite;

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
                return null;

            Rect rect = new Rect(0f, 0f, texture.width, texture.height);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
        }

        private void SetDigitSize(RectTransform rect, Sprite sprite)
        {
            float width = digitHeight;
            if (sprite != null && sprite.rect.height > 0f)
                width = digitHeight * (sprite.rect.width / sprite.rect.height);

            rect.sizeDelta = new Vector2(width, digitHeight);

            LayoutElement layout = rect.GetComponent<LayoutElement>();
            if (layout == null)
                layout = rect.gameObject.AddComponent<LayoutElement>();

            layout.preferredWidth = width;
            layout.preferredHeight = digitHeight;
            layout.minWidth = width;
            layout.minHeight = digitHeight;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;
        }
    }
}
