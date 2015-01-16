namespace BingDailyPicture.Models
{
    class BingImage
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        /// <remarks>Example: <c>/az/hprichbg/rb/KampaIsland_ROW8812407324_1366x768.jpg</c></remarks>
        public string Url { get; set; }

        //"urlbase":"",
        /// <summary>
        /// Gets or sets the URL base.
        /// </summary>
        /// <value>
        /// The URL base.
        /// </value>
        /// <remarks>Example: <c>/az/hprichbg/rb/KampaIsland_ROW8812407324</c></remarks>
        public string UrlBase { get; set; }
    }
}