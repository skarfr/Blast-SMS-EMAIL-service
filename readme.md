# Blast

Blast is a windows service used to send messages (SMS, emails) located in a database, through any kind of web api.
It has been succesfully tested and used with 3 different providers (so far):
* onewaysms.com.my : SMS provider in Malaysia
* vht.com.vn : SMS provider in Vietnam
* mandrill.com : Emailing provider

### How does it work?
* Every minutes, Blast will query the table MessagesOut and look for unsent messages (aka `SentTime == null`)
* Using the Providers table data, Blast will query the related web api URL and send/submit the message accordingly
* Finally, Blast will parse the provider http response, and match it versus the ProvidersStatus data, so you can easily track how the provider handled your request

### How to install?
#### Database & Build
* I assume you got Visual Studio and SQL Server
* Refer to `db/Blast database.sql` and `db/Example data.sql` to create the database
* Add to your database a user with the Database role membership `db_owner` and the Grant Permission `connect`
* You may want to change the connexion string named `BlastConnectionString` in Solution >> Model >> Properties >> Settings.settings
* Make sure you got dotNet 3.5. Then, you can Build the solution
#### Service
* Copy all built files to the location `C:\Blast\`
* Run As Administrator `C:\Windows\System32\cmd.exe`
```
cd C:\Windows\Microsoft.NET\Framework\v2.0.50727
InstallUtil.exe "C:\Blast\Blast.exe"
```
* In "Administrative Tools", open "Computer Managment"
* Check that the service named 'Blast' is started and in 'Automatic' Startup Type.

### How to uninstall?
* Run As Administrator `C:\Windows\System32\cmd.exe`
```
cd C:\Windows\Microsoft.NET\Framework\v2.0.50727
InstallUtil.exe /u "C:\Blast\Blast.exe"
```

### How do i send new messages?
* Everything happpen in the Blast database. The easiest way to create new messages is by doing SQL inserts.
* Example: To wish a Happy birthday to all your customers, you could create a daily SQL server job which will insert new messages based on their birth date.
* Among all MessagesOut columns, you must insert(at least) `Sender`, `Receiver`, `Message`, `MessageType` to send a new messages. MessageType accepts 2 values: SMS or EMAIL
* Make sure that your `MessageOutRow.SentTime == null`
* Make sure you got a matching Providers with `Enable == True`
* You do not have to restart Blast after each inserts. Indeed, Blast looks every minutes for new unsent messages and it tries to send them

[MessagesOut].Status can contains: 
* `send` (not case sentitive): ready to be send
* `sending` (not case sentitive): being send
* `notsent` (not case sentitive): message not sent: check logs for the source "Bast" in the Event Viewer
* `sent` (not case sentitive): message successfully sent

### What about Providers?
The table Provider can be tricky. You can find examples in `db/Example data.sql`

[Providers].RequestEncoding accepts 4 different values:
* `HttpUtility.UrlEncode`
* `Uri.EscapeDataString`
* `Uri.EscapeUriString`
* `None`
You can find more information in the file Tools.UrlParameters.cs

[Providers].MessageType accepts 2 different values:
* `SMS` (not case sentitive)
* `EMAIL` (not case sentitive)

[Providers].Url and [Providers].Parameters can contain variables from Blast through below keywords:
* `##receiver##` : The phone number or email we want to send to
* `##receivername##` : If email, receivername is the receiver name
* `##sender##` : The phone number, email, SMS Id (string) we send from
* `##sendername##` : If email, sendername is the sender name used (Sender Name <sender@mailserver.tld>). Can be null for SMS
* `##subject##` : If email, it's the email subject. Can be null for SMS
* `##message##` : the sms or email content to be send

[Providers].RequestMIMEType must be one MIME Types : http://en.wikipedia.org/wiki/MIME_type

### What about ProvidersStatus?
[ProvidersStatus].Action accepts:
* `send` (not case sentitive)
* `check` (not case sentitive)

### Troubleshooting
The service write logs in your computer Event Viewer under "Windows Logs" >> "Application"
The source is "Blast"

### Good to know
* Blast service will run every 1 minutes. It's normal if nothing happen after you run it, you must wait for 1 minute! This value can be changed by editing the constructor of srvcBlast.cs.
* Blast service used to run on a windows server 2003 with .net 3.5 and that's why I used a regex to double check that an email is correct. I realize that there are more elegant way to do this nowaday and this regex may not match future email patterns/tld. Feel free to update the function `Verif.Email()` to change this behavior
* If the receiver is a mobile phone, it must be a number without any special chars. For example, you can not use the `+` symbol for international blasting. It must be replaced with the international call prefix chosen by the country the call is being made from: https://en.wikipedia.org/wiki/List_of_international_call_prefixes
* Any MessagesOut with a `null` SentTime will be send.
