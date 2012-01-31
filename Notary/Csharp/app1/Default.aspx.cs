﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Services;
using System.Text;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public partial class _Default : System.Web.UI.Page
{
    string shortCode, FQDN, oauthFlow;
    string api_key, secret_key, auth_code, access_token, authorize_redirect_uri, scope, expiryMilliSeconds, refresh_token, lastTokenTakenTime, refreshTokenExpiryTime;
    Table successTable, failureTable;
    string amount;
    Int32 category;
    string channel, description, merchantTransactionId, merchantProductId, merchantApplicationId;
    Uri merchantRedirectURI;
    string paymentType;
    string signedPayLoad, signature, goBackURL;
    string MerchantSubscriptionIdList, SubscriptionRecurringPeriod;
    Int32 SubscriptionRecurringNumber, SubscriptionRecurringPeriodAmount;
    string IsPurchaseOnNoActiveSubscription;
    DateTime transactionTime;
    string transactionTimeString;
    string payLoadStringFromRequest;

    /* this function returns true, if ssl handshake is done with the connecting server */
    public static void BypassCertificateError()
    {
        ServicePointManager.ServerCertificateValidationCallback +=
            delegate(Object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    /* This function reads transaction configuration parameters from config file and stores locally */
    private void readTransactionParametersFromConfigurationFile()
    {
        transactionTime = DateTime.UtcNow;
        transactionTimeString = String.Format("{0:ddd-MMM-dd-yyyy-HH-mm-ss}", transactionTime);
        if (ConfigurationManager.AppSettings["Amount"] == null)
        {
            drawPanelForFailure(notaryPanel, "Amount is not defined in configuration file");
            return;
        }
        amount = ConfigurationManager.AppSettings["Amount"];
        requestText.Text = "Amount: " + amount + "\r\n";
        if (ConfigurationManager.AppSettings["Category"] == null)
        {
            drawPanelForFailure(notaryPanel, "Category is not defined in configuration file");
            return;
        }
        category = Convert.ToInt32(ConfigurationManager.AppSettings["Category"]);
        requestText.Text = requestText.Text + "Category: " + category + "\r\n";
        if (ConfigurationManager.AppSettings["Channel"] == null)
        {
            channel = "MOBILE_WEB";
        }
        else
        {
            channel = ConfigurationManager.AppSettings["Channel"];
        }
        requestText.Text = requestText.Text + "Channel: " + channel + "\r\n";
        description = "TrDesc" + transactionTimeString;
        requestText.Text = requestText.Text + "Description: " + description + "\r\n";
        merchantTransactionId = "TrId" + transactionTimeString;
        requestText.Text = requestText.Text + "MerchantTransactionId: " + merchantTransactionId + "\r\n";
        merchantProductId = "ProdId" + transactionTimeString;
        requestText.Text = requestText.Text + "MerchantProductId: " + merchantProductId + "\r\n";
        merchantApplicationId = "MerAppId" + transactionTimeString;
        requestText.Text = requestText.Text + "MerchantApplicationId: " + merchantApplicationId + "\r\n";
        if (ConfigurationManager.AppSettings["MerchantPaymentRedirectUrl"] == null)
        {
            drawPanelForFailure(notaryPanel, "MerchantPaymentRedirectUrl is not defined in configuration file");
            return;
        }
        merchantRedirectURI = new Uri(ConfigurationManager.AppSettings["MerchantPaymentRedirectUrl"]);
        requestText.Text = requestText.Text + "MerchantPaymentRedirectUrl: " + merchantRedirectURI;
    }

    /* This function reads subscription configuration parameters from config file and stores locally */
    private void readSubscriptionParametersFromConfigurationFile()
    {
        if (ConfigurationManager.AppSettings["MerchantSubscriptionIdList"] == null)
        {
            MerchantSubscriptionIdList = "merSubIdList" + transactionTimeString;
        }
        else
        {
            MerchantSubscriptionIdList = ConfigurationManager.AppSettings["MerchantSubscriptionIdList"];
        }
        requestText.Text = requestText.Text + "\r\n" + "MerchantSubscriptionIdList: " + MerchantSubscriptionIdList + "\r\n";
        if (ConfigurationManager.AppSettings["SubscriptionRecurringPeriod"] == null)
        {
            SubscriptionRecurringPeriod = "MONTHLY";
        }
        else
        {
            SubscriptionRecurringPeriod = ConfigurationManager.AppSettings["SubscriptionRecurringPeriod"];
        }
        requestText.Text = requestText.Text + "SubscriptionRecurringPeriod: " + SubscriptionRecurringPeriod + "\r\n";
        if (ConfigurationManager.AppSettings["SubscriptionRecurringNumber"] == null)
        {
            SubscriptionRecurringNumber = Convert.ToInt32("9999");
        }
        else
        {
            SubscriptionRecurringNumber = Convert.ToInt32(ConfigurationManager.AppSettings["SubscriptionRecurringNumber"]);
        }
        requestText.Text = requestText.Text + "SubscriptionRecurringNumber: " + SubscriptionRecurringNumber + "\r\n";
        if (ConfigurationManager.AppSettings["SubscriptionRecurringPeriodAmount"] == null)
        {
            SubscriptionRecurringPeriodAmount = Convert.ToInt32("1");
        }
        else
        {
            SubscriptionRecurringPeriodAmount = Convert.ToInt32(ConfigurationManager.AppSettings["SubscriptionRecurringPeriodAmount"]);
        }
        requestText.Text = requestText.Text + "SubscriptionRecurringPeriodAmount: " + SubscriptionRecurringPeriodAmount + "\r\n";
        if (ConfigurationManager.AppSettings["IsPurchaseOnNoActiveSubscription"] == null)
        {
            IsPurchaseOnNoActiveSubscription = "false";
        }
        else
        {
            IsPurchaseOnNoActiveSubscription = ConfigurationManager.AppSettings["IsPurchaseOnNoActiveSubscription"];
        }
        requestText.Text = requestText.Text + "IsPurchaseOnNoActiveSubscription: " + IsPurchaseOnNoActiveSubscription;
    }

    /* This function is called when applicaiton is getting loaded */

    protected void Page_Load(object sender, EventArgs e)
    {
        BypassCertificateError();
        DateTime currentServerTime = DateTime.UtcNow;
        serverTimeLabel.Text = String.Format("{0:ddd, MMM dd, yyyy HH:mm:ss}", currentServerTime) + " UTC";
        FQDN = ConfigurationManager.AppSettings["FQDN"].ToString();
        if (ConfigurationManager.AppSettings["FQDN"] == null)
        {
            drawPanelForFailure(notaryPanel, "FQDN is not defined in configuration file");
            return;
        }
        FQDN = ConfigurationManager.AppSettings["FQDN"].ToString();
        if (ConfigurationManager.AppSettings["api_key"] == null)
        {
            drawPanelForFailure(notaryPanel, "api_key is not defined in configuration file");
            return;
        }
        api_key = ConfigurationManager.AppSettings["api_key"].ToString();
        if (ConfigurationManager.AppSettings["secret_key"] == null)
        {
            drawPanelForFailure(notaryPanel, "secret_key is not defined in configuration file");
            return;
        }
        secret_key = ConfigurationManager.AppSettings["secret_key"].ToString();
        if (ConfigurationManager.AppSettings["scope"] == null)
        {
            scope = "PAYMENT";
        }
        else
        {
            scope = ConfigurationManager.AppSettings["scope"].ToString();
        }
        if ((Request["signed_payload"] != null) && (Request["signed_signature"] != null)
            && (Request["goBackURL"] != null) && (Request["signed_request"] != null))
        {
            signPayLoadButton.Text = "Back";
            requestText.Text = Request["signed_request"].ToString();
            SignedPayLoadTextBox.Text = Request["signed_payload"].ToString();
            SignatureTextBox.Text = Request["signed_signature"].ToString();
            goBackURL = Request["goBackURL"].ToString();
        }
        else if ((Request["request_to_sign"] != null) && (Request["goBackURL"] != null)
                  && (Request["api_key"] != null) && (Request["secret_key"] != null))
        {
            payLoadStringFromRequest = Request["request_to_sign"].ToString();
            goBackURL = Request["goBackURL"].ToString();
            SignedPayLoadTextBox.Text = payLoadStringFromRequest.ToString();
            api_key = Request["api_key"].ToString();
            secret_key = Request["secret_key"].ToString();
            executeSignedPayloadFromRequest();
        }
        else
        {
            if (ConfigurationManager.AppSettings["paymentType"] == null)
            {
                drawPanelForFailure(notaryPanel, "paymentType is not defined in configuration file");
                return;
            }
            paymentType = ConfigurationManager.AppSettings["paymentType"];
            if (paymentType.Equals("Transaction", StringComparison.OrdinalIgnoreCase))
            {
                readTransactionParametersFromConfigurationFile();
            }
            else if (paymentType.Equals("Subscription", StringComparison.OrdinalIgnoreCase))
            {
                readTransactionParametersFromConfigurationFile();
                readSubscriptionParametersFromConfigurationFile();
            }
            else
            {
                drawPanelForFailure(notaryPanel, "paymentType is  defined with invalid value in configuration file.  Valid values are Transaction or Subscription.");
                return;
            }
            string payLoadString = "{'Amount':'" + amount.ToString() + "','Category':'" + category.ToString() + "','Channel':'" +
                        channel.ToString() + "','Description':'" + description.ToString() + "','MerchantTransactionId':'"
                        + merchantTransactionId.ToString() + "','MerchantProductId':'" + merchantProductId.ToString()
                        + "','MerchantApplicaitonId':'" + merchantApplicationId.ToString() + "','MerchantPaymentRedirectUrl':'"
                        + merchantRedirectURI.ToString() + "','MerchantSubscriptionIdList':'" + MerchantSubscriptionIdList.ToString()
                        + "','IsPurchaseOnNoActiveSubscription':'" + IsPurchaseOnNoActiveSubscription.ToString()
                        + "','SubscriptionRecurringNumber':'" + SubscriptionRecurringNumber.ToString()
                        + "','SubscriptionRecurringPeriod':'" + SubscriptionRecurringPeriod.ToString()
                        + "','SubscriptionRecurringPeriodAmount':'" + SubscriptionRecurringPeriodAmount.ToString();
            requestText.Text = payLoadString.ToString();
        }
    }

    /* this function is called with other app call notary applicaiton to sign the payload */
    public void executeSignedPayloadFromRequest()
    {
        try
        {
            string sendingData = payLoadStringFromRequest.ToString();
            String newTransactionResponseData;
            WebRequest newTransactionRequestObject = (WebRequest)System.Net.WebRequest.Create("" + FQDN + "/Security/Notary/Rest/1/SignedPayload?client_id=" + api_key.ToString() + "&client_secret=" + secret_key.ToString());
            newTransactionRequestObject.Method = "POST";
            newTransactionRequestObject.ContentType = "application/json";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(sendingData);
            newTransactionRequestObject.ContentLength = postBytes.Length;

            Stream postStream = newTransactionRequestObject.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            WebResponse newTransactionResponseObject = (HttpWebResponse)newTransactionRequestObject.GetResponse();
            using (StreamReader newTransactionResponseStream = new StreamReader(newTransactionResponseObject.GetResponseStream()))
            {
                newTransactionResponseData = newTransactionResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                TransactionResponse deserializedJsonObj = (TransactionResponse)deserializeJsonObject.Deserialize(newTransactionResponseData, typeof(TransactionResponse));
                newTransactionResponseStream.Close();
                //SignedPayLoadTextBox.Text = deserializedJsonObj.SignedDocument.ToString();
                //SignatureTextBox.Text = deserializedJsonObj.Signature.ToString();
                Response.Redirect(goBackURL.ToString() + "?ret_signed_payload=" + deserializedJsonObj.SignedDocument.ToString() + "&ret_signature=" + deserializedJsonObj.Signature.ToString());
            }
        }
        catch (Exception ex)
        {
            //SignatureTextBox.Text = ex.ToString();
            //Response.Redirect(goBackURL.ToString() + "?ret_signed_payload_failed=true");
        }
    }

    /* this function is called by the user clicke button function  to sign the payload */
    public bool executeSignedPayload()
    {
        try
        {
            String newTransactionResponseData;
            WebRequest newTransactionRequestObject = (WebRequest)System.Net.WebRequest.Create("" + FQDN + "/Security/Notary/Rest/1/SignedPayload?client_id=" + api_key.ToString() + "&client_secret=" + secret_key.ToString());
            string payLoadString = "{'Amount':'" + amount.ToString() + "','Category':'" + category.ToString() + "','Channel':'" +
                                    channel.ToString() + "','Description':'" + description.ToString() + "','MerchantTransactionId':'"
                                    + merchantTransactionId.ToString() + "','MerchantProductId':'" + merchantProductId.ToString()
                                    + "','MerchantApplicaitonId':'" + merchantApplicationId.ToString() + "','MerchantPaymentRedirectUrl':'"
                                    + merchantRedirectURI.ToString() + "','MerchantSubscriptionIdList':'" + MerchantSubscriptionIdList.ToString()
                                    + "','IsPurchaseOnNoActiveSubscription':'" + IsPurchaseOnNoActiveSubscription.ToString()
                                    + "','SubscriptionRecurringNumber':'" + SubscriptionRecurringNumber.ToString()
                                    + "','SubscriptionRecurringPeriod':'" + SubscriptionRecurringPeriod.ToString()
                                    + "','SubscriptionRecurringPeriodAmount':'" + SubscriptionRecurringPeriodAmount.ToString() + "'}";
            newTransactionRequestObject.Method = "POST";
            newTransactionRequestObject.ContentType = "application/json";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(payLoadString);
            newTransactionRequestObject.ContentLength = postBytes.Length;

            Stream postStream = newTransactionRequestObject.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            WebResponse newTransactionResponseObject = (HttpWebResponse)newTransactionRequestObject.GetResponse();
            using (StreamReader newTransactionResponseStream = new StreamReader(newTransactionResponseObject.GetResponseStream()))
            {
                newTransactionResponseData = newTransactionResponseStream.ReadToEnd();
                JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();
                TransactionResponse deserializedJsonObj = (TransactionResponse)deserializeJsonObject.Deserialize(newTransactionResponseData, typeof(TransactionResponse));
                SignedPayLoadTextBox.Text = deserializedJsonObj.SignedDocument.ToString();
                SignatureTextBox.Text = deserializedJsonObj.Signature.ToString();
                //Response.Redirect(redirectUrl.ToString());
                newTransactionResponseStream.Close();
                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    /* this function is called if user clicks on signPayLoad button */
    protected void signPayLoadButton_Click(object sender, EventArgs e)
    {
        if (signPayLoadButton.Text.Equals("Back", StringComparison.CurrentCultureIgnoreCase))
        {
            try
            {
                Response.Redirect(goBackURL.ToString() + "?shown_notary=true");
            }
            catch (Exception ex)
            {
                drawPanelForFailure(notaryPanel, ex.ToString());
            }
        }
        else
        {
            bool result = executeSignedPayload();
        }
    }

    /* this function draws the failure result */

    private void drawPanelForFailure(Panel panelParam, string message)
    {
        failureTable = new Table();
        failureTable.Font.Name = "Sans-serif";
        failureTable.Font.Size = 9;
        failureTable.BorderStyle = BorderStyle.Outset;
        failureTable.Width = Unit.Pixel(650);
        TableRow rowOne = new TableRow();
        TableCell rowOneCellOne = new TableCell();
        rowOneCellOne.Font.Bold = true;
        rowOneCellOne.Text = "ERROR:";
        rowOne.Controls.Add(rowOneCellOne);
        //rowOneCellOne.BorderWidth = 1;
        failureTable.Controls.Add(rowOne);
        TableRow rowTwo = new TableRow();
        TableCell rowTwoCellOne = new TableCell();
        //rowTwoCellOne.BorderWidth = 1;
        rowTwoCellOne.Text = message.ToString();
        rowTwo.Controls.Add(rowTwoCellOne);
        failureTable.Controls.Add(rowTwo);
        failureTable.BorderWidth = 2;
        failureTable.BorderColor = Color.Red;
        failureTable.BackColor = System.Drawing.ColorTranslator.FromHtml("#fcc");
        panelParam.Controls.Add(failureTable);
    }
}

/* Following are the data structures used for the application */

public class AccessTokenResponse
{
    public string access_token;
    public string refresh_token;
    public string expires_in;
}

public class TransactionResponse
{
    public string SignedDocument
    {
        get;
        set;
    }
    public string Signature
    {
        get;
        set;
    }
}