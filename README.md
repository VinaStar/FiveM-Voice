# FiveM-Voice

### FEATURES:
- 3 range modes (Whisper, Default, Shout)
- Switch range mode using Shift+H (Controller Sprint + RightDpad)
- View currently speaking players
- Server side range settings configurable
  
---
  
### REQUIREMENTS:
- [Vina Framework](https://github.com/VinaStar/Vina-Framework/releases)
  
---
  
### EXPORTS:
- setEnabled(bool)  
**Toggle the module on or off**  
  
- setVisibility(bool)  
**Toggle the HUD visibility on or off**  
  
-- setVoiceLevel(int)  
**Set the voice level 0 = Whisper, 1 = Default, 2 = Shout**  
  
---
  
### CONVARS:
You can change the settings using convar in your FiveM server config file:
   
- *set fivem_voice_level_whisper 5*  
**Set the range of Whispering to 5 meters**

- *set fivem_voice_level_default 25*  
**Set the range of Default to 25 meters**

- *set fivem_voice_level_shout 50*  
**Set the range of Shouting to 50 meters**
