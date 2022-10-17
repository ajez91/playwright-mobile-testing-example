using Microsoft.Playwright;
using System.Text;
using System.Text.RegularExpressions;

namespace CouponFollowTests.Pages
{
    public class HomePage
    {
        private IPage Page;
        private string SearchDiscountTitle = " Discount Codes";
        public HomePage(IPage page) => Page = page;

        #region Locators
        private ILocator TopOfferBullet => Page.Locator("span.bullet");
        private ILocator TopCouponActive => Page.Locator(".swiper-slide-active");
        private ILocator TopCouponNext => Page.Locator(".swiper-slide-next");
        private ILocator TopCouponPrevious => Page.Locator(".swiper-slide-prev");
        private ILocator TrendingCouponMobile => Page.Locator(".trending-mobile");
        private ILocator StaffPick => Page.Locator(".staff-pick");
        private ILocator StaffPickMerchant => Page.Locator(".staff-pick span.merch");
        private ILocator StaffPickTitle => Page.Locator(".staff-pick p.title");
        private ILocator SearchBox => Page.Locator(".search-btn");
        private ILocator MobileSearchBoxInput => Page.Locator(".mobile-search-input");
        private ILocator MobileSearchSugestion => Page.Locator(".mobile-suggestion-item");
        private ILocator MobileSearchResultTitle => Page.Locator("#site > h1");
        private ILocator MobileSearchResultDiscounts => Page.Locator("#deals");

        #endregion Locators

        #region Methods
        private async Task<int> GetTopCouponsBullets() => await TopOfferBullet.CountAsync();
        private async Task<int> GetTrendingCouponMobile() => await TrendingCouponMobile.CountAsync();
        private async Task<int> GetStaffPicks() => await StaffPick.CountAsync();
        private async Task ClickSearchBox() => await SearchBox.ClickAsync();
        private async Task InputValueInMobileSearchBox(string value) => await MobileSearchBoxInput.TypeAsync(value, new() { Delay = 100 });
        private async Task ChooseSearchSugestionMobile() => await MobileSearchSugestion.ClickAsync();
        private async Task<string> GetMobileSearchResultTitle() => await MobileSearchResultTitle.TextContentAsync();
        public async Task GoToUrl(string url) => await Page.GotoAsync(url);
        public async Task SearchForDiscountMobile(string merchant)
        {
            await ClickSearchBox();
            await InputValueInMobileSearchBox(merchant);
            await ChooseSearchSugestionMobile();
        }

        #endregion Methods

        #region Validations

        private async Task<bool> ValidateIfPreviousTopCouponIsDisplayed() => await TopCouponPrevious.IsVisibleAsync();
        private async Task<bool> ValidateIfActiveTopCouponIsDisplayed() => await TopCouponActive.IsVisibleAsync();
        private async Task<bool> ValidateIfNextTopCouponIsDisplayed() => await TopCouponNext.IsVisibleAsync();

        public async Task<bool> ValidateThat3or6or9TopCouponsArePrepared()
        {

            var offers = await GetTopCouponsBullets();
            return offers == 3 || offers == 6 || offers == 9;
        }

        public async Task<bool> Validate3TopCouponsAreDisplayed()
        {
            List<bool> bools = new List<bool>();

            bools.Add(await ValidateIfPreviousTopCouponIsDisplayed());
            bools.Add(await ValidateIfActiveTopCouponIsDisplayed());
            bools.Add(await ValidateIfNextTopCouponIsDisplayed());

            return bools.Count(b=>b) == 3;
        }

        public async Task<bool> ValidateNumberOfDisplayedTodaysTrendingCoupons()
        {
            List<bool> bools = new List<bool>();

            var trendingCoupons = await GetTrendingCouponMobile();
            for (int i = 0; i < trendingCoupons; i++)
            {
                bools.Add(await TrendingCouponMobile.Nth(i).IsVisibleAsync());
            }

            return bools.Count(b => b) >= 30;
        }

        public async Task<bool> ValidateStaffPicksUniqueStores()
        {
            List<string> stores = new List<string>();

            var staffPicks = await GetStaffPicks();
            for (int i = 0; i < staffPicks; i++)
            {
                stores.Add(await StaffPickMerchant.Nth(i).TextContentAsync());
            }

            return stores.Distinct().Count() == stores.Count();
        }

        public async Task<bool> ValidateStaffPicksProperDiscounts()
        {
            List<string> discountTitles = new List<string>();
            
            var staffPicks = await GetStaffPicks();
            for (int i = 0; i < staffPicks; i++)
            {
                discountTitles.Add(await StaffPickTitle.Nth(i).TextContentAsync());
            }

            //Verifying discounts for monetary, percentage or text values
            Regex pattern = new Regex(@"\$\d+|\d+\%|\w+");
            var filterResults = discountTitles.Select(d => pattern.IsMatch(d)).ToList();

            return discountTitles.Count() == filterResults.Count(); 
        }

        public async Task<bool> ValidateMobileSearchDiscountResultAreDisplayed() => await MobileSearchResultDiscounts.IsVisibleAsync();

        public async Task<bool> ValidateMobileSearchDiscountResultPageTitle(string merchant)
        {
            StringBuilder sb = new StringBuilder(SearchDiscountTitle);
            sb.Insert(0, merchant);

            return sb.ToString() == await GetMobileSearchResultTitle();
        }

        #endregion Validations
    }
}
