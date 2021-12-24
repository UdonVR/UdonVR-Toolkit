# Easy Doors
## How to use
* (Import the UdonVR-Toolkit Package from [here](https://github.com/VrcUdon/UdonVR-Toolkit/releases/tag/2.2) if not already done)
* Create your Door model. This can be anything, as **long as it has a collider**. Place the model in your scene.
* Add a `Udon Behaviour` component to your model.
* Drag the `PlayerInteractTeleport` UdonSharp script into the `Program Source`  option.
* Create a new empty, and place it where the player should spawn after entering your door.
* Drag the new empty into the `Target Logation` field.
* If you want, you can also change the `Target Position`, `Target Rotation` and `Target Orientation` (I recommend leaving it as default) properties to match your world.
* Optional: Select all events when you want the player to teleport. 
    - `Nothing` disables the door.
    - `Everything` enables every event.
    - `On Interact` means that the player will have to click on the door to get teleported.
    - `On Enter` means that the player will get teleported when they enter the **collider**[^1].
    - `On Exit` means that the player will get teleported when they leave the **collider**[^1] (eg. they walked "through" the door).
* Optional: Enable Lerp to remote. It means that other players will see the teleporting player "Move" to the Target Location instead of instantly teleporting there.

[^1]: You will have to set the collider to `Is Trigger` for that to work. Note, that this will disable the actual collision of the collider, so your player may be able to walk through the door object.
