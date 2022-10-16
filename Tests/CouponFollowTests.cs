using CouponFollowTests.Pages;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace CouponFollowTests.Tests
{
    public class CouponFollowTests : PageTest
    {
        private HomePage homePage;

        [SetUp]
        public async Task Setup()
        {
            homePage = new HomePage(Page);
            await homePage.GoToUrl("https://couponfollow.com/");
        }

        [Test, Category("Mobile")]
        [TestCase(TestName = "Validate That 3 Out Of 3 Or 6 Or 9 Top Deal Coupons Are Displayed - IPhone")]
        public async Task Validate_That_3_Out_Of_3_Or_6__Or_9_Top_Deal_Coupons_Are_Aisplayed_IPhone_Test()
        {

            Assert.Multiple(async () =>
            {
                Assert.IsTrue(await homePage.ValidateThat3or6or9TopCouponsArePrepared(),
                    "Number of prepared Top Coupons are other than expected 3 or 6 or 9");
                Assert.IsTrue(await homePage.Validate3TopCouponsAreDisplayed(),
                    "Number of visible Coupons is other than expected");
            });
        }

        [Test, Category("Mobile")]
        [TestCase(TestName = "Validate At Least 30 Today's Trending Coupons Are Displayed - IPhone")]
        public async Task Validate_At_Least_30_Todays_Trending_Coupons_Are_Displayed_IPhone_Test()
        {
            Assert.IsTrue(await homePage.ValidateNumberOfDisplayedTodaysTrendingCoupons(),
                "Number of displayed Today's Trending Coupons is less than expected");
        }

        [Test, Category("Mobile")]
        [TestCase(TestName = "Validate Staff Picks Unique Stores And Proper Discounts - IPhone")]
        public async Task Validate_Staff_Picks_Unique_Stores_And_Proper_Discounts_IPhone_Test()
        {
            Assert.Multiple(async () =>
            {
                Assert.IsTrue(await homePage.ValidateStaffPicksUniqueStores(),
                "Stores in Staff Picks are not unique");
                Assert.IsTrue(await homePage.ValidateStaffPicksProperDiscounts(),
                    "Discounts title in Staff Picks are not proper");
            });
        }

        [Test, Category("Mobile")]
        [TestCase("StubHub", TestName = "Validate Searching For Discount - IPhone")]
        public async Task Validate_Searching_For_Discount_IPhone_Test(string merchant)
        {
            await homePage.SearchForDiscountMobile(merchant);

            Assert.Multiple(async () =>
            {
                Assert.IsTrue(await homePage.ValidateMobileSearchDiscountResultPageTitle(merchant),
                    "Mobile search discount results page title is not correct");
                Assert.IsTrue(await homePage.ValidateMobileSearchDiscountResultAreDisplayed(),
                    "Mobile search discount results are not displayed correctly");
            });
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions(Playwright.Devices["iPhone 12"]);
        }
    }
}
