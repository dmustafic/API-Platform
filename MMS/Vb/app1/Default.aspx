<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en"><head>
	<title>AT&T Sample MMS Application 1 &#8211; Basic MMS Service Application</title>
	<meta content="text/html; charset=ISO-8859-1" http-equiv="Content-Type"/>
    <link rel="stylesheet" type="text/css" href="../../style/common.css"/>
    <style type="text/css">
        .style1
        {
            font-style: normal;
            font-variant: normal;
            font-weight: bold;
            font-size: 12px;
            line-height: normal;
            font-family: Arial, Sans-serif;
            width: 92px;
        }
    </style>
    </head>
<body>
<div id="container">
<!-- open HEADER --><div id="header">
<div>
    <div id="hcLeft">Server Time:</div>
       	<div id="hcRight">
            <asp:Label ID="serverTimeLabel" runat="server" Text="Label"></asp:Label>
        </div>
    </div>
<div>
    <div id="hcLeft">Client Time:</div>
	<div id="hcRight">
        <script language="JavaScript" type="text/javascript">
            var myDate = new Date();
            document.write(myDate);
        </script>
    </div>
</div>
<div>
    <div id="hcLeft">User Agent:</div>    
	<div id="hcRight">
        <script language="JavaScript" type="text/javascript">
            document.write("" + navigator.userAgent);
        </script>
    </div>
</div>

<br clear="all" />
</div><!-- close HEADER -->

<div id="wrapper">
<div id="content">

<h1>AT&T Sample MMS Application 1 � Basic MMS Service Application</h1>
<h2>Feature 1: Send MMS Message</h2>

</div>
</div>

<br clear="all" />
<form id="form1" runat="server">           
<div id="navigation">

<table border="0" width="100%">
  <tbody>
  <tr>
    <td width="20%" valign="top" class="label">Phone:</td>
    <td class="cell">
        <asp:TextBox ID="phoneTextBox" runat="server" Width="199px" MaxLength="16"></asp:TextBox>     
            </td>
  </tr> 
  <tr>
    <td valign="top" class="label">Message:</td>
    <td class="cell">
        <asp:TextBox ID="messageTextBox" runat="server" Height="99px" 
            TextMode="MultiLine" Width="291px"></asp:TextBox></td>
  </tr>
  </tbody>
</table>
</div>

<div id="extra">
<div class="warning">
<strong>WARNING:</strong><br />
total size of all attachments cannot exceed 600 KB.
</div>
<table border="0" width="100%">
  <tbody>
  <tr>
    <td valign="bottom" class="style1">Attachment 1:</td>
    <td class="cell"><asp:FileUpload ID="FileUpload1" runat="server"/>
    </td>
  </tr>
  <tr>
    <td valign="bottom" class="style1">Attachment 2:</td>
    <td class="cell"><asp:FileUpload ID="FileUpload2" runat="server"/>
    </td>
  </tr>
  <tr>
    <td valign="bottom" class="style1">Attachment 3:</td>
    <td class="cell"><asp:FileUpload ID="FileUpload3" runat="server"/>
    </td>
  </tr>
  </tbody></table>
  <table>
  <tbody>
  <tr>
  	<td>
          <asp:Button ID="sendMMSMessageButton" runat="server" Text="Send MMS Message" 
              onclick="sendMMSMessageButton_Click" /></td>
  </tr>
  </tbody></table>
</div>
<br clear="all" />
<div align="center">
    <asp:Panel ID="sendMessagePanel" runat="server" Font-Names="Calibri"  Font-Size="XX-Small">
    </asp:Panel></div>
<br clear="all" />

<div id="wrapper">
<div id="content">

<h2><br />
Feature 2: Get Delivery Status</h2>

</div>
</div>
<div id="navigation">

<table border="0" width="100%">
  <tbody>
  <tr>
    <td width="20%" valign="top" class="label">Message ID:</td>
    <td class="cell">
        <asp:TextBox ID="messageIDTextBox" runat="server" Width="233px"></asp:TextBox>
    </td>
  </tr>
  </tbody></table>
  
</div>
<div id="extra">

<table border="0" width="100%">
  <tbody>
  <tr>
    <td class="cell">
        <asp:Button ID="getStatusButton" runat="server" Text="GetStatus" 
            onclick="getStatusButton_Click" />
    </td>
  </tr>
  </tbody></table>

</div>
<br clear="all" />
<div align="center">
    <asp:Panel ID="getStatusPanel" runat="server" Font-Names="Calibri" Font-Size="XX-Small">
    </asp:Panel>
</div>
<br clear="all" />




<div id="footer">

	<div style="float: right; width: 20%; font-size: 9px; text-align: right">Powered by AT&amp;T Virtual Mobile</div>
    <p>� 2011 AT&amp;T Intellectual Property. All rights reserved.  <a href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
<br>
The Application hosted on this site are working examples intended to be used for reference in creating products to consume AT&amp;T Services and  not meant to be used as part of your product.  The data in these pages is for test purposes only and intended only for use as a reference in how the services perform.
<br>
For download of tools and documentation, please go to <a href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
<br>
For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a>

</div>
</form>

</body></html>