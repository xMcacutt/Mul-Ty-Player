# Customization

---

There are a few parts of the MTP client which can be modified externally to allow customization such as custom sounds, backgrounds, and colors. All customizable files are in the GUI directory in the client directory.

---

## *NOTE\* - Prior to v3.4.3, the updater overwrote these files with the default versions with every update. This behaviour will be altered to allow customization but your updater must be upgraded to v3.4.3 or later by installing from github if you wish to customize the default files and not have them be overwritten.

---

## Themes

Themes allow you to modify all of the colors used in the client. In the Themes folder, you'll find two json files. All colors used by the client are defined by these files. The client searches this folder for json files so additional themes can be added and the default themes can be modified. 

A standard theme json files requires the following definition

```json
{
  "MainBackColor": "#AARRGGBB",
  "AltBackColor": "#AARRGGBB",
  "SpecialBackColor": "#AARRGGBB",
  "MainTextColor": "#AARRGGBB",
  "AltTextColor": "#AARRGGBB",
  "InvertedTextColor": "#AARRGGBB",
  "MainAccentColor": "#AARRGGBB",
  "AltAccentColor": "#AARRGGBB"
}
```

Each color uses ARGB hex code notation.

The below image shows how the colors above are used in the lobby UI to help with visualising your customization.

--- 

## Sounds

Swapping out the sounds is even easier than making custom themes. Simple locate the Sounds directory and note down the name of the sound you want to swap. Then replace it with your desired sound ensuring the name is identical to the original.

---

## Graphics

The final customizable aspect of the client is the koala selection images and the Ty theme background used on the splash screen and koala select. It's unlikely you'd want to change these assets but all are available in the Koala Selection Assets directory and can be swapped similarly to the Sounds.
