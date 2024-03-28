# /HideSeek

## Basic Information

#### Description
Activate/Deactivate Hide & Seek mode, stop the current Hide & Seek session, or adjust the hit detection range

#### Aliases
- hs

#### Usages
- /hideseek \<true/false>
- /hideseek abort
- /hideseek range \<x>

#### Arguments
- \<true/false> - Whether Hide & Seek mode should be turned on or off.
- \<x> - Floating point range for hit detection.

#### IsHostOnly?
- True

#### CanSpectatorRun?
- True

## More Information
The hideseek command can be used to activate or deactivate the mode similar to the functionality of the host menu button.
It also has unique features in its ability to abort a running hide and seek session or change the hit detection range. 
A hit detection range of around 80 is reasonable. For more information on the mode, see [the HideSeek Mode documentation](../HideSeek.md).
This mode can only be activated or deactivated by a [host](../Host) which may or may not be a spectator.