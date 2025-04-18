# [![Tag Engine](https://github.com/harrydoestechonyt/tag-engine/blob/main/github/tagenginelogo.png?raw=true)]()

A free, powerful and open-source Tag Engine for Gorilla Tag fangames

[![Download](https://img.shields.io/badge/Download-blue.svg)](https://github.com/harrydoestechonyt/tag-engine/releases/latest)
[![Total Downloads](https://img.shields.io/github/downloads/harrydoestechonyt/tag-engine/total.svg)]()

## IMPORTANT!
If you encounter any bugs, please create a new issue [here](https://github.com/harrydoestechonyt/tag-engine/issues/new).  
Be sure to include details such as your Unity version, error logs, and steps to reproduce the bug.



# Contributing

Feel free to fork the repository, create issues, or submit pull requests. Contributions are welcome to make TagEngine even better!

# Setting up

**This assumes you have already have PhotonVR and the other stuff set up.**

## Credits

If you use this Tag Engine in your game, please give credit to the original author, **HarryDoesTech**.  
Credits should be displayed in the game’s credits screen, documentation, or other prominent places.

Example credit:
> **Tag Engine by HarryDoesTech**  
> For more information, visit [GitHub Profile](https://github.com/harrydoestechonyt)

This is required under the terms of the MIT License.


## Dependencies
To set up, make sure you have these packages installed:

[![PhotonVR](https://github.com/fchb1239/PhotonVR/blob/main/Visuals/SmallerText.png)](https://github.com/fchb1239/PhotonVR/releases)

[Gorilla Locomotion](https://github.com/Another-Axiom/GorillaLocomotion/blob/main/GorillaLocomotion.unitypackage)

[Photon PUN](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922?srsltid=AfmBOoroqdAGQOi15SQeyHhB87O4HQ0Q4JMXaO3-MDkTOPz6KYk7m06P) and
[Photon Voice](https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518)

**Follow the installation instructions for these dependencies to ensure the engine works seamlessly within your Gorilla Tag fangame.**

## Adding to the game

Watch the video here on how to add it:

[![How to Set Up](https://img.youtube.com/vi/x2COQWejvJY/0.jpg)](https://youtu.be/x2COQWejvJY)


### !!NEW UPDATE!!!
Due to the new update, make sure to assign "Untagged" to TagEngine/Resources/Materials/Untagged.mat



# Scripting using the engine

In all scripts you plan to use this engine, make sure to put

```cs
using HarryDoesTechStudios.TagEngine;
```

in any scripts where you need to use the engine (i.e Tag Zone)

example script:
```cs
using HarryDoesTechStudios.TagEngine;
using UnityEngine;
using Photon.Pun;

public class ExampleScript : MonoBehaviour{
  public string TagHitBox;

  //Just tags on hit

  void OnTriggerEnter(Collider other){
     if(other.tag == TagHitBox){
       PhotonView target = other.GetComponent<PhotonView>();
       if(target != null){
          TagHitbox hitbox = target.GetComponent<TagHitbox>();
          if(hitbox != null && !hitbox.isTag){
            target.RPC(nameof(hitbox.OnHit), RpcTarget.AllBuffered, "a tag zone.");
          }
       }
    }
  }
}
```
