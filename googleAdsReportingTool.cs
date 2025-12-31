// Google Ads Reporting Tool
// -------------------------
// This file is sanitized for public sharing.
// Replace placeholders like [YOUR_DEVELOPER_TOKEN], [YOUR_LOGIN_CUSTOMER_ID], [YOUR_CUSTOMER_ID],
// and [YOUR_DIRECTORY_PATH] with your actual values in App.config or code before running.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoogleAdsReportGenerator;
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V17.Services;
using Google.Ads.GoogleAds.V17.Errors;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using System.Globalization;
using Google.Ads.GoogleAds.V17.Common;
using System.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

using System.Threading;
using Google.Apis.Auth.OAuth2.Responses;

namespace GoogleAdsReportGenerator
{
    public partial class Form1 : Form
    {
        private const string ADWORDS_API_SCOPE = "https://www.googleapis.com/auth/adwords";
        
        string directoryPath = ConfigurationManager.AppSettings["ReportOutputDirectory"];

        public Form1()
        {
            InitializeComponent();
        }

        private GoogleAdsClient GetGoogleAdsClientDetails()
        {
            Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string OAuth2ClientId = appConfig.AppSettings.Settings["OAuth2ClientId"].Value;
            string OAuth2ClientSecret = appConfig.AppSettings.Settings["OAuth2ClientSecret"].Value;
            string OAuth2RefreshToken = appConfig.AppSettings.Settings["OAuth2RefreshToken"].Value;

            GoogleAdsConfig config = new GoogleAdsConfig()
            {
                DeveloperToken = appConfig.AppSettings.Settings["DeveloperToken"].Value,
                OAuth2Mode = 0,
                OAuth2ClientId = OAuth2ClientId,
                OAuth2ClientSecret = OAuth2ClientSecret,
                OAuth2RefreshToken = OAuth2RefreshToken,

                LoginCustomerId = appConfig.AppSettings.Settings["LoginCustomerId"].Value
            };

            GoogleAdsClient client = new GoogleAdsClient(config);
            return client;
        }

        private Dictionary<long, string> GetAllActiveCampaigns(GoogleAdsClient client, long customerId)
        {

            // Get the GoogleAdsService.
            GoogleAdsServiceClient googleAdsService = client.GetService(
                Services.V17.GoogleAdsService);

            // Create a query that will retrieve all campaigns.
            string query = @"SELECT
                    campaign.id,
                    campaign.name,
                    campaign.network_settings.target_content_network
                FROM campaign
                WHERE campaign.status = 'ENABLED'
                ORDER BY campaign.id";

            try
            {
                // Issue a search request.
                googleAdsService.SearchStream(customerId.ToString(), query,
                    delegate (SearchGoogleAdsStreamResponse resp)
                    {
                        //MessageBox.Show(resp.Results.Count.ToString());
                        foreach (GoogleAdsRow googleAdsRow in resp.Results)
                        {
                            CampaignList.Add(googleAdsRow.Campaign.Id, googleAdsRow.Campaign.Name);
                        }
                    }
                );
            }
            //Google.Apis.Auth.OAuth2.Responses.TokenResponseException: 
            catch (Exception e)
            {
                MessageBox.Show("Authentication failed. Please refresh credentials.", "Token Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
           //MessageBox.Show(CampaignList.Count.ToString());
            return CampaignList;
        }


        private string GetOrCreateDirectory(string dirPath)
        {
            try
            {
                if (Directory.Exists(dirPath))
                {
                    return new DirectoryInfo(dirPath).FullName;
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(dirPath);
                    return di.FullName;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("The Proecess of Creating Failed: {0}", ex.Message.ToString());
                return null;
            }
            finally { }

        }

        private string GetFullFilePath(string reportName)
        {
            string fullFilePath = "";

            string dirPath = GetOrCreateDirectory(directoryPath);
            string fileName = "ClickData_" + reportName + "_createdOn_" + System.DateTime.Today.ToString("dd_MMM_yyyy")+".txt";
            fullFilePath = Path.Combine(dirPath, fileName);
            return fullFilePath;
        }

        private void GetKeyWordsAdsWithClicks(GoogleAdsClient client, long customerId, long campaignId, StreamWriter sw)
        {

            // Get the GoogleAdsService.
            GoogleAdsServiceClient googleAdsService = client.GetService(
                Services.V17.GoogleAdsService);

            // Create a query that will retrieve all campaigns.
            string dt = dp_SpecificDate.Value.Date.ToString("yyyy-MM-dd");
            string query = @"SELECT 
                            ad_group.name, 
                            campaign.name,
                            ad_group_criterion.keyword.text,
                            metrics.clicks,
                            metrics.impressions, 
                            metrics.interactions, 
                            segments.year,
                            segments.month_of_year,
                            segments.date
                            from keyword_view where metrics.clicks > 0 and segments.date BETWEEN @START_DATE AND @END_DATE and campaign.id ='" + campaignId.ToString() +"'";
                            //segments.year='2024' and segments.month_of_year='NOVEMBER' and campaign.id ='" + campaignId.ToString() +"'";

            try
            {
                // Issue a search request.
                googleAdsService.SearchStream(customerId.ToString(), query,
                    delegate (SearchGoogleAdsStreamResponse resp)
                    {
                        //MessageBox.Show(resp.Results.Count.ToString());
                        foreach (GoogleAdsRow googleAdsRow in resp.Results)
                        {
                            //string dt = googleAdsRow.Segments.Date.ToString();
                            //System.DateTime dtfull = System.DateTime.ParseExact(dt, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            
                            //use this //ClickDate\tCampaignYear\tCampaignName\tAdGroup\tKeyword\tClicks\tImpressions\tInteractions\tIsJunkPart
                            
                            //string headers = "Date\tYear\tCampaignName\tAdGroup\tKeyword\tClicks\tInteractions\tImpressions";
                            string result = googleAdsRow.Segments.Date + "\t" + googleAdsRow.Segments.Year + "\t" + googleAdsRow.Campaign.Name + "\t" + googleAdsRow.AdGroup.Name + "\t" + googleAdsRow.AdGroupCriterion.Keyword.Text +
                                            "\t" + googleAdsRow.Metrics.Clicks + "\t" + googleAdsRow.Metrics.Interactions + "\t" + googleAdsRow.Metrics.Impressions;
                            sw.WriteLine(result);
                            
                        }
                    }
                );
            }
            catch (GoogleAdsException e)
            {
                MessageBox.Show(e.Message);
                //Console.WriteLine("Failure:");
                //Console.WriteLine($"Message: {e.Message}");
                //Console.WriteLine($"Failure: {e.Failure}");
                //Console.WriteLine($"Request ID: {e.RequestId}");
                throw;
            }
        }

        private void GenerateReqportQueryAndFile(string swFileName, string query)
        {
            Dictionary<long, string> CampaignList = new Dictionary<long, string>();
            GoogleAdsClient client = GetGoogleAdsClientDetails();
            long custId = long.Parse(ConfigurationManager.AppSettings["GoogleAdsCustomerId"]);
            string originalQuery = query;

            CampaignList = GetAllActiveCampaigns(client, custId);
            if (CampaignList.Count > 0)
            {
                string headers = "Date\tCampaignName\tAdGroup\tKeyword\tClicks\tInteractions\tImpressions";
                using (StreamWriter sw = new StreamWriter(swFileName, false))
                {
                    sw.WriteLine(headers);
                    foreach (var de in CampaignList)
                    {
                        long campaignId = de.Key;
                        string tempQuery = originalQuery + " and campaign.id ='" + campaignId.ToString() + "'";
                        GetKeywordsReport(client, custId, campaignId, sw, tempQuery);
                    }
                }
                sw.Close();
                CampaignList.Clear();
                MessageBox.Show("Report completed");
            }

        }

        private void GetKeywordsReport(GoogleAdsClient client, long customerId, long campaignId, StreamWriter sw, string query)
        {
            GoogleAdsServiceClient googleAdsService = client.GetService(
               Services.V17.GoogleAdsService);

            
            try
            {
                // Issue a search request.
                googleAdsService.SearchStream(customerId.ToString(), query,
                    delegate (SearchGoogleAdsStreamResponse resp)
                    {
                        //MessageBox.Show(resp.Results.Count.ToString());
                        foreach (GoogleAdsRow googleAdsRow in resp.Results)
                        {
                            //string dt = googleAdsRow.Segments.Date.ToString();
                            //System.DateTime dtfull = System.DateTime.ParseExact(dt, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                           
                            //string headers = "Date\tYear\tCampaignName\tAdGroup\tKeyword\tClicks\tInteractions\tImpressions";
                            string result = googleAdsRow.Segments.Date + "\t" + googleAdsRow.Campaign.Name + "\t" + googleAdsRow.AdGroup.Name + "\t" + googleAdsRow.AdGroupCriterion.Keyword.Text +
                                            "\t" + googleAdsRow.Metrics.Clicks + "\t" + googleAdsRow.Metrics.Interactions + "\t" + googleAdsRow.Metrics.Impressions;
                            sw.WriteLine(result);
                        }
                    }
                );
            }
            catch (GoogleAdsException e)
            {
                MessageBox.Show(
                    "Authentication failed or token expired. Please refresh the token.",
                    "Google Ads Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        
        
        private void btnGetDateReport_Click(object sender, EventArgs e)
        {
            this.lblTokenCreated.Visible = false;
            string selectedDateForQuery = dp_SpecificDate.Value.ToString("yyyy-MM-dd");
            string selectedDateForFile = dp_SpecificDate.Value.ToString("dd-MMM-yyyy");
            string fullFile = GetFullFilePath("ReportByDate_" + selectedDateForFile);

            string query = @"SELECT 
                            ad_group.name, 
                            campaign.name,
                            ad_group_criterion.keyword.text,
                            metrics.clicks,
                            metrics.impressions, 
                            metrics.interactions, 
                            segments.year,
                            segments.month_of_year,
                            segments.date
                            from keyword_view where metrics.clicks > 0 and segments.date ='" + selectedDateForQuery + "'";

            GenerateReqportQueryAndFile(fullFile, query);
        }

        private void btnGetMonthReport_Click(object sender, EventArgs e)
        {
            this.lblTokenCreated.Visible = false;
            string selectedMonthForQuery = dp_SpecificMonth.Value.ToString("MMMM").ToUpper();
            string selectedYearForQuery = dp_SpecificMonth.Value.ToString("yyyy");
            string selectedMonthYearForFile = dp_SpecificMonth.Value.ToString("MMM-yyyy");
            string fullFile = GetFullFilePath("ReportByMonth_" + selectedMonthYearForFile);

            string query = @"SELECT 
                            ad_group.name, 
                            campaign.name,
                            ad_group_criterion.keyword.text,
                            metrics.clicks,
                            metrics.impressions, 
                            metrics.interactions, 
                            segments.year,
                            segments.month_of_year,
                            segments.date
                            from keyword_view where metrics.clicks > 0 and
                            segments.year='" + selectedYearForQuery + "' and segments.month_of_year='" + selectedMonthForQuery + "'";
            
            GenerateReqportQueryAndFile(fullFile, query);
        }

        private void btnMonthRange_Click(object sender, EventArgs e)
        {
            this.lblTokenCreated.Visible = false;
            int selectedFromMonth = Int32.Parse(dp_FromMonth.Value.ToString("MM"));
            int selectedFromYear = Int32.Parse(dp_FromMonth.Value.ToString("yyyy"));
            int selectedToMonth = Int32.Parse(dp_ToMonth.Value.ToString("MM"));
            int selectedToYear = Int32.Parse(dp_ToMonth.Value.ToString("yyyy"));
            


            System.DateTime date1 = new System.DateTime(selectedFromYear, selectedFromMonth, 1, 0, 0, 0);
            int daysinToMonth = System.DateTime.DaysInMonth(selectedToYear, selectedToMonth);

            System.DateTime date2 = new System.DateTime(selectedToYear, selectedToMonth, daysinToMonth, 0, 0, 0);
            if (date1 > date2)
            {
                MessageBox.Show("From Month is later than the To Month!");
                return; ;
            }

           

            //string selectedFromMonthForQuery = dp_FromMonth.Value.ToString("MMMM").ToUpper();
            //string selectedToMonthForQuery = dp_ToMonth.Value.ToString("MMMM").ToUpper();
            //string selectedFromYearForQuery = dp_FromMonth.Value.ToString("yyyy");
            //string selectedToYearForQuery = dp_ToMonth.Value.ToString("yyyy");

            string selectedMonthYearForFile = dp_FromMonth.Value.ToString("MMM-yyyy")+ "to"+ dp_ToMonth.Value.ToString("MMM-yyyy");
            string fullFile = GetFullFilePath("ReportByMonth_" + selectedMonthYearForFile);

            string FromDateForQuery =date1.ToString("yyyy-MM-dd");
            string ToDateForQuery = date2.ToString("yyyy-MM-dd");

            string query = @"SELECT 
                            ad_group.name, 
                            campaign.name,
                            ad_group_criterion.keyword.text,
                            metrics.clicks,
                            metrics.impressions, 
                            metrics.interactions, 
                            segments.year,
                            segments.month_of_year,
                            segments.date
                            from keyword_view where metrics.clicks > 0 and segments.date BETWEEN '"+ FromDateForQuery+"' AND '"+ ToDateForQuery +"'";

            GenerateReqportQueryAndFile(fullFile, query);

        }

        private void btnRefreshToken_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string OAuth2ClientId = config.AppSettings.Settings["OAuth2ClientId"].Value;
            string OAuth2ClientSecret = config.AppSettings.Settings["OAuth2ClientSecret"].Value;

            ClientSecrets secrets = new ClientSecrets()
            {
                ClientId = OAuth2ClientId,
                ClientSecret = OAuth2ClientSecret
            };

            List<string> scopes = new List<string>();
            scopes.Add(ADWORDS_API_SCOPE);

            try
            {
                // Authorize the user using installed application flow.
                Task<UserCredential> task = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets,
                    scopes, String.Empty, CancellationToken.None, new NullDataStore());
                task.Wait();
                UserCredential credential = task.Result;

                // NOTE: In production, refresh tokens should be stored securely
                // (e.g., encrypted store or secrets manager).
                // Token persistence is intentionally omitted in this public version.

                this.lblTokenCreated.Visible = true;
                //OAuth2ClientId
                //OAuth2ClientSecret
                //OAuth2RefreshToken
            }
            catch (AggregateException)
            {
                Console.WriteLine("An error occured while authorizing the user.");
            }
        }

    }
}