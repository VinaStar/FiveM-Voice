# Vina-Voice

### FEATURES:
- 3 range modes (Whisper, Default, Shout)
- Can use Proximity or Global Channels
- Can customize UI with CSS
- Can switch voice range using *Shift+H* (Controller *Sprint + RightDpad*)
- Can see currently speaking players
- Can configure resource in the server.cfg
- Can use exports methods with your own resources
  
---
  
**Client Performance**  
![Client Memory Usage](https://i.imgur.com/BjJ1isu.png)  
**Server Performance**  
![Client Memory Usage](https://i.imgur.com/sZlnrWQ.png)  
**Server Console Output**  
![Server Console Output](https://i.imgur.com/r3eYRmC.png)  
  
---
  
### DEPENDENCIES:
- [Vina Framework](https://github.com/VinaStar/Vina-Framework/releases)
  
---
   
### INSTRUCTIONS:
   
   **1)** Place "vinavoice" directory inside your server Resources directory.
   
   **2)** Add "ensure vinavoice" to your server config.
   
   **3)** Start your FiveM server.
   
---
  
### CLIENT EXPORTS:
- getEnabled()  
**return:** bool  
**info:** Get if the module is on or off  
  
- setEnabled(**bool** enabled)  
**info:** Toggle the module on or off  
  
- getVisibility()  
**return:** bool  
**info:** Get if the module is visible or not  
  
- setVisibility(**bool** visible)  
**info:** Toggle the HUD visibility on or off  
  
- getVoiceLevel()  
**return:** int  
**info:** Get the voice level, 0 = Whisper, 1 = Default, 2 = Shout  
  
- setVoiceLevel(**int** level)  
**info:** Set the voice level, 0 = Whisper, 1 = Default, 2 = Shout  
  
- getVoiceChannel()  
**return:** int  
**info:** Get the voice channel -1 is proximity, >= 0 is channels  
  
- setVoiceChannel(**int** channel)  
**info:** Set the voice channel -1 is proximity, >= 0 is channels  
  
---
  
### SERVER EXPORTS:
- getEnabled(**string** playerHandle)  
**return:** bool  
**info:** Get if the module is on or off  
  
- setEnabled(**string** playerHandle, **bool** enabled)  
**info:** Toggle the module on or off  
  
- getVoiceLevel(**string** playerHandle)  
**return:** int  
**info:** Get the voice level, 0 = Whisper, 1 = Default, 2 = Shout  
  
- setVoiceLevel(**string** playerHandle, **int** level)  
**info:** Set the voice level, 0 = Whisper, 1 = Default, 2 = Shout  
  
- getVoiceChannel(**string** playerHandle)  
**return:** int  
**info:** Get the voice channel -1 is proximity, >= 0 is channels  
  
- setVoiceChannel(**string** playerHandle, **int** channel)  
**info:** Set the voice channel -1 is proximity, >= 0 is channels  
  
---
  
### CONVARS:
You can change the settings using convar in your FiveM server config file:
   
- *set vina_voice_whisper_proximity_distance 5*  
**Set the range of Whispering to 5 meters**

- *set vina_voice_default_proximity_distance 25*  
**Set the range of Default to 25 meters**

- *set vina_voice_shout_proximity_distance 50*  
**Set the range of Shouting to 50 meters**
