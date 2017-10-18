# Hank Hill
A discord bot for pixelating and jpegifying images

<a href="https://discordapp.com/oauth2/authorize?client_id=323123443136593920&scope=bot&permissions=67184707"><img src="https://img.shields.io/badge/Add%20to%20your-Discord-9399ff.svg" alt="Invite to your Guild"></a> [![Discord](https://discordapp.com/api/guilds/326668996550197249/widget.png)](https://discord.gg/ebXZnFX)

# Usage
1. Sending "needs more jpeg" (or "more jpg") will jpegify the last image in the current Channel
2. Sending "pixelate" will pixelate the last image in the current Channel
3. Sending "@HankHill help" will show help

<p align="center">
	<img src="https://raw.githubusercontent.com/mrousavy/HankHill/master/Resources/Hank.png" />
	<br />
	<img src="https://raw.githubusercontent.com/mrousavy/HankHill/master/Resources/Demo.png" />
</p>

# Building yourself
### Prerequisites
1. [Visual Studio](https://www.visualstudio.com/)
2. [.NET Core SDK](https://www.microsoft.com/net/download/core)

### Build & Run
1. Download/Clone this Project
2. Open it in Visual Studio
3. Go to `Tools > NuGet Package Manager > Package Manager Settings > Package Sources`
4. Add a new Source: "ImageSharp Beta", URL: https://www.myget.org/F/imagesharp/api/v3/index.json
5. Click `Update`, close dialog with `Ok`
6. Change Project output from `Debug` to `Release` mode in drop down menu
7. Build Project (<kbd>Ctrl</kbd><kbd>Shift</kbd><kbd>B</kbd>, or `Build > Build Solution`)

If step **7** was successful, open the Project in your file explorer and go to `HankHill/bin/Release`. Your executable will be located in a subfolder here.

Run `HankHill.exe`. It should fail at the first time because we need a **token**.
Go to the [discord API interface](https://discordapp.com/developers/applications/me) and create a new app. [This guide](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token) explains it quite well.

Open the file `token` in any text editor and paste the discord API token in there. Save and run `HankHill.exe`, it should now start successfully.

Add the bot to your server with this link: https://discordapp.com/oauth2/authorize?client_id=CLIENT_ID_GOES_HERE&scope=bot

Now you're good to go.
