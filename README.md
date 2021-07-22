# Tarkov Loader + Mods

This project consists of several minor [modifications](#Mods) for Escape From Tarkov, as well as [Loaders](#Loaders) for both SPT-AKI and Melonloader.

## Loaders

### SPT-AKI - [Homepage](https://www.sp-tarkov.com/)

Extract the downloaded folder into `<SPT-AKI>/user/mods/`. Extract each mod in the same way, so that all of the `.dll` files end up in the same folder.

### MelonLoader - [Homepage](https://melonwiki.xyz/#/)

Extract the downloaded files into `<Tarkov>/Mods/`. Extract each mod in the same way, so that all of the `.dll` files end up in the same folder.

## Mods

### ItemValue

Adds an attribute to items that display when examining them, this value shows the full price of the item to traders.

**Examples**

M4 - Price including all attachments, mag, ammo, etc.

![M4](./Screenshots/ItemValue%20-%20M4%20Example.png)

Buckshot - Price for whole stack.

![alt text](./Screenshots/ItemValue%20-%20Ammo%20Example.png)

### Extract

Adds the ability to press a key and extract from a raid. Was built as I encountered an issue in SPT-AKI that stopped the extraction points from registering a player entering them. Useful for debugging and messing around.

### Freecam

Detaches the camera from the player and lets them fly around. Flycam funnctionality shamelessly repurposed from the main game.

**Controls**

- Mouse - Look around

- WASD - Move around

- Q/E - Move Up/Down

- Shift (hold) - Speed up (3x)

- Control (hold) - Slow down (0.25x)

- End or Escape - (Un)lock cursor

### Nightvision

Adds the ability to toggle the ingame Nightvision overlay on/off.