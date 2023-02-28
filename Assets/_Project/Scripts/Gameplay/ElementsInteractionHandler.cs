using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ElementTypes
{
    Fire,
    Water,
    Electric,
    Rock,
    Wind,
    Steam,
    Lava,
    Explosive,
    Plasma,
    Plants,
    Ice,
    Laser,
    Dust,
    Magnetism,
    Storm,
    Void,
    Life,
    Death,
    None
}

public enum ElementLevel
{
    Level0,
    Level1,
    Level2,
    Level3
}

public class ElementsInteractionHandler : MonoBehaviour
{
    [SerializeField]
    ElementTypes _elementType = ElementTypes.None;

    [SerializeField]
    ElementLevel _elementLevel = ElementLevel.Level0;

    private void OnParticleCollision(GameObject other)
    {
        ElementsInteractionHandler elementsInteractionHandler = other.GetComponent<ElementsInteractionHandler>();
        if (elementsInteractionHandler != null)
        {
            HandleElementInteraction(elementsInteractionHandler._elementType, elementsInteractionHandler._elementLevel, other);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Colisiono contra: " + other.gameObject);

        //if (other.gameObject.layer != _soundTrigger))//Why it doesnt work this way?
        if (other.gameObject.layer != LayerMask.NameToLayer("EnvironmentSoundTrigger") && other.gameObject.tag != "IgnoreCollider")
        {
            if (gameObject == other.gameObject)
                return;

            if (other.GetComponent<ElementsInteractionHandler>() != null)
            {
                ElementsInteractionHandler interactingElement = other.GetComponent<ElementsInteractionHandler>();
                HandleElementInteraction(interactingElement._elementType, interactingElement._elementLevel, other.gameObject);
            }
        }
    }

    public void HandleElementInteraction(ElementTypes elementType, ElementLevel elementLevel, GameObject collidingObject)
    {
        Debug.Log("Objeto que fue colisionado: " + gameObject);
        Debug.Log("Colisiono contra: " + collidingObject);

        bool collidingObjectIsItem = false;
        bool alreadyDrainedEnergy = false;

        switch (_elementType, elementType)//first the this gameobject own element, second incoming object element
        {
            #region //Fire interactions

            case (ElementTypes.Fire, ElementTypes.Fire)://Fire up
                break;

            case (ElementTypes.Fire, ElementTypes.Water)://Evaporarates water and produces steam

                GameObject steamPrefab1 = AssetsDatabaseManager._instance.elementRections[(int)ElementTypes.Steam];
                if (steamPrefab1 != null)
                {
                    GameObject instatiatedSteam = Instantiate(steamPrefab1, transform.position, Quaternion.identity, transform.parent);
                    Destroy(instatiatedSteam, 8);
                }

                if (collidingObject.GetComponentInParent<PickableItem>())//If the object colliding is not an item it can be destroyed, just decrease gems energy
                {
                    float energySpent = 10 * (int)elementLevel;//Define how much energy it takes to do stuff in this case to put out the fire depending on its level
                    PickableItem pickableItem = collidingObject.GetComponentInParent<PickableItem>();
                    Item item = pickableItem.item;
                    DrainGemEnergySpentInInteraction(item, energySpent);
                    collidingObjectIsItem = true;
                    alreadyDrainedEnergy = true;
                }

                if (collidingObject.GetComponentInParent<GemsInObjectsManager>() && alreadyDrainedEnergy == false)//If the object colliding is not an item it can be destroyed and the energy hasnt been drained yet, just decrease gems energy
                {
                    float energySpent = 10 * (int)elementLevel;//Define how much energy it takes to do stuff in this case to put out the fire depending on its level
                    GemsInObjectsManager gemsInObjectsManager = collidingObject.GetComponentInParent<GemsInObjectsManager>();
                    DrainGemEnergySpentInInteraction(gemsInObjectsManager, energySpent);
                    collidingObjectIsItem = true;
                }

                if (collidingObjectIsItem == false )
                {
                    Destroy(collidingObject);
                }

                break;

            case (ElementTypes.Fire, ElementTypes.Electric)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Rock)://Leaves a smoky decal, if enough heat is applied it turns into lava(maybe with tier 3 fire)
                break;

            case (ElementTypes.Fire, ElementTypes.Wind)://Fire tornado? Explosion?
                break;

            case (ElementTypes.Fire, ElementTypes.Steam)://Clears up the fog
                break;

            case (ElementTypes.Fire, ElementTypes.Lava)://If solid, mealts it up again
                break;

            case (ElementTypes.Fire, ElementTypes.Explosive)://if is a dormant explosive makes it explode, otherwise ?(explosions last seconds soo not much of a time window to interact with a live explosion)
                break;

            case (ElementTypes.Fire, ElementTypes.Plasma)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Plants)://Burns the plants and leaves charcoal maybe?
                break;

            case (ElementTypes.Fire, ElementTypes.Ice)://Melts the ice and produces water
                break;

            case (ElementTypes.Fire, ElementTypes.Laser)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Dust)://If is a cloud of dust/the dust is suspended in the air it generates an explosion otherwise turns it into a crystal/mirror surface?
                break;

            case (ElementTypes.Fire, ElementTypes.Magnetism)://Nothing, or could it make it stronger or weaker?, enable it or disable it?
                break;

            case (ElementTypes.Fire, ElementTypes.Storm)://Subdues the fire, sooo nothing really maybe the play the steam FX to convey the fire is being put out?
                break;

            case (ElementTypes.Fire, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Water intereactions

            case (ElementTypes.Water, ElementTypes.Fire)://Puts out the fire and evaporates producing steam

                //GameObject steamPrefab2 = AssetsDatabaseManager._instance.elementRections[(int)ElementTypes.Steam];
                //if (steamPrefab2 != null)
                //{
                //    GameObject instatiatedSteam = Instantiate(steamPrefab2, transform.position, Quaternion.identity, transform.parent);
                //    if (gameObject.GetComponentInChildren<PickableItem>())//If the object colliding is not an item it can be destroyed, just decrease gems energy
                //    {
                //        float energySpent = 10;//Define how much energy it takes to do stuff in this case to put out the fire depending on its level
                //        PickableItem pickableItem = gameObject.GetComponentInChildren<PickableItem>();
                //        Item item = pickableItem.item;
                //        DrainGemEnergySpentInInteraction(item, energySpent);
                //    }
                //    else
                //    {
                //        Destroy(gameObject);
                //    }
                //    Destroy(instatiatedSteam, 8);
                //}

                GameObject steamPrefab2 = AssetsDatabaseManager._instance.elementRections[(int)ElementTypes.Steam];
                if (steamPrefab2 != null)
                {
                    GameObject instatiatedSteam = Instantiate(steamPrefab2, transform.position, Quaternion.identity, transform.parent);
                    Destroy(instatiatedSteam, 8);
                }

                if (collidingObject.GetComponentInParent<PickableItem>())//If the object colliding is not an item it can be destroyed, just decrease gems energy
                {
                    float energySpent = 10 * (int)elementLevel;//Define how much energy it takes to do stuff in this case to put out the fire depending on its level
                    PickableItem pickableItem = collidingObject.GetComponentInParent<PickableItem>();
                    Item item = pickableItem.item;
                    DrainGemEnergySpentInInteraction(item, energySpent);
                    collidingObjectIsItem = true;
                    alreadyDrainedEnergy = true;
                }

                if (collidingObject.GetComponentInParent<GemsInObjectsManager>() && alreadyDrainedEnergy == false)//If the object colliding is not an item it can be destroyed and the energy hasnt been drained yet, just decrease gems energy
                {
                    float energySpent = 10 * (int)elementLevel;//Define how much energy it takes to do stuff in this case to put out the fire depending on its level
                    GemsInObjectsManager gemsInObjectsManager = collidingObject.GetComponentInParent<GemsInObjectsManager>();
                    DrainGemEnergySpentInInteraction(gemsInObjectsManager, energySpent);
                    collidingObjectIsItem = true;
                }

                if (collidingObjectIsItem == false)
                {
                    Destroy(collidingObject);
                }

                break;

            case (ElementTypes.Water, ElementTypes.Water)://More water, adds water
                break;

            case (ElementTypes.Water, ElementTypes.Electric)://Electrifies the water
                break;

            case (ElementTypes.Water, ElementTypes.Rock)://Wet stone decal or wet shader?
                break;

            case (ElementTypes.Water, ElementTypes.Wind)://Water tornado, storm?
                break;

            case (ElementTypes.Water, ElementTypes.Steam)://Clears up the fog
                break;

            case (ElementTypes.Water, ElementTypes.Lava)://Cools and solidifies it
                break;

            case (ElementTypes.Water, ElementTypes.Explosive)://if is a dormant explosive ruins it so it no longer explodes even if it interact with fire, otherwise ?(explosions last seconds soo not much of a time window to interact with a live explosion)
                break;

            case (ElementTypes.Water, ElementTypes.Plasma)://Electrifies the water
                break;

            case (ElementTypes.Water, ElementTypes.Plants)://Makes the plants grow
                break;

            case (ElementTypes.Water, ElementTypes.Ice)://Freezes the water
                break;

            case (ElementTypes.Water, ElementTypes.Laser)://Electrifies the water
                break;

            case (ElementTypes.Water, ElementTypes.Dust)://Makes mud? or cleans the dust?
                break;

            case (ElementTypes.Water, ElementTypes.Magnetism)://Nothing
                break;

            case (ElementTypes.Water, ElementTypes.Storm)://Makes the storm stronger?
                break;

            case (ElementTypes.Water, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Water, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Water, ElementTypes.Death)://Produces poison?
                break;
            #endregion

            #region //Electric Interactions

            case (ElementTypes.Electric, ElementTypes.Fire)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Water)://Electrifies the water
                break;

            case (ElementTypes.Electric, ElementTypes.Electric)://Makes the electricity effect last longer or maybe recharges it?
                break;

            case (ElementTypes.Electric, ElementTypes.Rock)://Nothing, smoky decal?
                break;

            case (ElementTypes.Electric, ElementTypes.Wind)://Electric storm?
                break;

            case (ElementTypes.Electric, ElementTypes.Steam)://Nothing, clears the fog?
                break;

            case (ElementTypes.Electric, ElementTypes.Lava)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Explosive)://if is a dormant explosive makes it explode, otherwise ?(explosions last seconds soo not much of a time window to interact with a live explosion)
                break;

            case (ElementTypes.Electric, ElementTypes.Plasma)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Plants)://Burns the plants and leaves charcoal maybe?
                break;

            case (ElementTypes.Electric, ElementTypes.Ice)://Melts the ice and produces water
                break;

            case (ElementTypes.Electric, ElementTypes.Laser)://Nothing, changes its path/way?
                break;

            case (ElementTypes.Electric, ElementTypes.Dust)://Nothing, elctrostatic cloud of dust?
                break;

            case (ElementTypes.Electric, ElementTypes.Magnetism)://Could it make it stronger or weaker?, enable it or disable it?
                break;

            case (ElementTypes.Electric, ElementTypes.Storm)://Electric storm
                break;

            case (ElementTypes.Electric, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Rock Interactions

            case (ElementTypes.Rock, ElementTypes.Fire)://Puts out the fire
                break;

            case (ElementTypes.Rock, ElementTypes.Water)://Nothing?, creates mud?
                break;

            case (ElementTypes.Rock, ElementTypes.Electric)://Nothing, grounds the electricity?
                break;

            case (ElementTypes.Rock, ElementTypes.Rock)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Wind)://Nothing, calms the wind
                break;

            case (ElementTypes.Rock, ElementTypes.Steam)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Lava)://Starts to melt but gradually
                break;

            case (ElementTypes.Rock, ElementTypes.Explosive)://Cracks/Breaks the rock
                break;

            case (ElementTypes.Rock, ElementTypes.Plasma)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Plants)://Destroys/Cuts/Smash plants
                break;

            case (ElementTypes.Rock, ElementTypes.Ice)://Crushs/Cracks/Breaks ice
                break;

            case (ElementTypes.Rock, ElementTypes.Laser)://Turns the rock into lava
                break;

            case (ElementTypes.Rock, ElementTypes.Dust)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Magnetism)://Nothing, insulates from magnetic fields? 
                break;

            case (ElementTypes.Rock, ElementTypes.Storm)://Calms/stops the storm
                break;

            case (ElementTypes.Rock, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Wind Interactions
            case (ElementTypes.Wind, ElementTypes.Fire)://puts out the fire/kindles it(depends on the level maybe level 1 kindles it but level 3 puts the fire out)
                break;

            case (ElementTypes.Wind, ElementTypes.Water)://Nothing, moves the water?
                break;

            case (ElementTypes.Wind, ElementTypes.Electric)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Rock)://Nothing, if is strong enough maybe push/cuts rocks?
                break;

            case (ElementTypes.Wind, ElementTypes.Wind)://More wind
                break;

            case (ElementTypes.Wind, ElementTypes.Steam)://Blows away the fog
                break;

            case (ElementTypes.Wind, ElementTypes.Lava)://Cools it down but significantly slower tahan water or ice
                break;

            case (ElementTypes.Wind, ElementTypes.Explosive)://If its dormant nothing otherwise maybe blow the fumes from the explosion?
                break;

            case (ElementTypes.Wind, ElementTypes.Plasma)://Nothing, maybe electrify the air?
                break;

            case (ElementTypes.Wind, ElementTypes.Plants)://Moves the plants, maybe if ii is strong enough blows away the plants like on a wall or the ground
                break;

            case (ElementTypes.Wind, ElementTypes.Ice)://Nothing, if is strong enough maybe push/cuts ice blocks
                break;

            case (ElementTypes.Wind, ElementTypes.Laser)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Dust)://Blows off the dust
                break;

            case (ElementTypes.Wind, ElementTypes.Magnetism)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Storm)://Makes it stronger, if strong enough maybe blows it or moves it?
                break;

            case (ElementTypes.Wind, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Steam Interactions

            case (ElementTypes.Steam, ElementTypes.Fire)://Disappears when touching fire
                break;

            case (ElementTypes.Steam, ElementTypes.Water)://Sublimates(Disappears) when touching water
                break;

            case (ElementTypes.Steam, ElementTypes.Electric)://Nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Rock)://Nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Wind)://Is blown away by the wind
                break;

            case (ElementTypes.Steam, ElementTypes.Steam)://More steam
                break;

            case (ElementTypes.Steam, ElementTypes.Lava)://Nothing, maybe cools the lava a little?
                break;

            case (ElementTypes.Steam, ElementTypes.Explosive)://If dormant ruins the explosives otherwise nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Plasma)://Electrified cloud of steam
                break;

            case (ElementTypes.Steam, ElementTypes.Plants)://Nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Ice)://makes snow?
                break;

            case (ElementTypes.Steam, ElementTypes.Laser)://Nothing, cool visual effect of the laser illuminating the cloud of steam?
                break;

            case (ElementTypes.Steam, ElementTypes.Dust)://Nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Magnetism)://Nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Storm)://Nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Steam, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Lava Interactions

            case (ElementTypes.Lava, ElementTypes.Fire)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Water)://Evaporates the water but cools down the lava
                break;

            case (ElementTypes.Lava, ElementTypes.Electric)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Rock)://Melts the rock gradually
                break;

            case (ElementTypes.Lava, ElementTypes.Wind)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Steam)://Clears the fog
                break;

            case (ElementTypes.Lava, ElementTypes.Lava)://Adds lava
                break;

            case (ElementTypes.Lava, ElementTypes.Explosive)://If dormant it explodes, otherwise nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Plasma)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Plants)://Burns the plants
                break;

            case (ElementTypes.Lava, ElementTypes.Ice)://Melts the ice
                break;

            case (ElementTypes.Lava, ElementTypes.Laser)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Dust)://Melts the dust like rocks
                break;

            case (ElementTypes.Lava, ElementTypes.Magnetism)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Storm)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Lava, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Explosive Interactions

            case (ElementTypes.Explosive, ElementTypes.Fire)://If dormant explodes otherwise nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Water)://If dormant ruins it otherwise nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Electric)://If dormant explodes otherwise nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Rock)://Cracks/breaks the rock
                break;

            case (ElementTypes.Explosive, ElementTypes.Wind)://Nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Steam)://If dormant ruins it otherwise nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Lava)://If dormant explodes otherwise nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Explosive)://KATAPUNCHIS
                break;

            case (ElementTypes.Explosive, ElementTypes.Plasma)://If dormant explodes otherwise nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Plants)://Blows and burns the plants
                break;

            case (ElementTypes.Explosive, ElementTypes.Ice)://Breaks/melts a little
                break;

            case (ElementTypes.Explosive, ElementTypes.Laser)://Nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Dust)://Blows the dust away
                break;

            case (ElementTypes.Explosive, ElementTypes.Magnetism)://Nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Storm)://Blows the storm?
                break;

            case (ElementTypes.Explosive, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Explosive, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Plasma Interactions

            case (ElementTypes.Plasma, ElementTypes.Fire)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Water)://Water gets electrified
                break;

            case (ElementTypes.Plasma, ElementTypes.Electric)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Rock)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Wind)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Steam)://Clears the fog?, electrifies the fog?
                break;

            case (ElementTypes.Plasma, ElementTypes.Lava)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Explosive)://If dormant explodes otherwise nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Plasma)://Nothing, more charge to the plasma?
                break;

            case (ElementTypes.Plasma, ElementTypes.Plants)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Ice)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Laser)://Nothing, changes way/path?
                break;

            case (ElementTypes.Plasma, ElementTypes.Dust)://Nothing, Electroestatic cloud of dust?
                break;

            case (ElementTypes.Plasma, ElementTypes.Magnetism)://Mess up the polarity?
                break;

            case (ElementTypes.Plasma, ElementTypes.Storm)://Electric storm?
                break;

            case (ElementTypes.Plasma, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Plasma, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Plants Interactions

            case (ElementTypes.Plants, ElementTypes.Fire):
                break;

            case (ElementTypes.Plants, ElementTypes.Water):
                break;

            case (ElementTypes.Plants, ElementTypes.Electric):
                break;

            case (ElementTypes.Plants, ElementTypes.Rock):
                break;

            case (ElementTypes.Plants, ElementTypes.Wind):
                break;

            case (ElementTypes.Plants, ElementTypes.Steam):
                break;

            case (ElementTypes.Plants, ElementTypes.Lava):
                break;

            case (ElementTypes.Plants, ElementTypes.Explosive):
                break;

            case (ElementTypes.Plants, ElementTypes.Plasma):
                break;

            case (ElementTypes.Plants, ElementTypes.Plants):
                break;

            case (ElementTypes.Plants, ElementTypes.Ice):
                break;

            case (ElementTypes.Plants, ElementTypes.Laser):
                break;

            case (ElementTypes.Plants, ElementTypes.Dust):
                break;

            case (ElementTypes.Plants, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Plants, ElementTypes.Storm):
                break;

            case (ElementTypes.Plants, ElementTypes.Void):
                break;

            case (ElementTypes.Plants, ElementTypes.Life):
                break;

            case (ElementTypes.Plants, ElementTypes.Death):
                break;
            #endregion

            #region //Ice Interactions

            case (ElementTypes.Ice, ElementTypes.Fire):
                break;

            case (ElementTypes.Ice, ElementTypes.Water):
                break;

            case (ElementTypes.Ice, ElementTypes.Electric):
                break;

            case (ElementTypes.Ice, ElementTypes.Rock):
                break;

            case (ElementTypes.Ice, ElementTypes.Wind):
                break;

            case (ElementTypes.Ice, ElementTypes.Steam):
                break;

            case (ElementTypes.Ice, ElementTypes.Lava):
                break;

            case (ElementTypes.Ice, ElementTypes.Explosive):
                break;

            case (ElementTypes.Ice, ElementTypes.Plasma):
                break;

            case (ElementTypes.Ice, ElementTypes.Plants):
                break;

            case (ElementTypes.Ice, ElementTypes.Ice):
                break;

            case (ElementTypes.Ice, ElementTypes.Laser):
                break;

            case (ElementTypes.Ice, ElementTypes.Dust):
                break;

            case (ElementTypes.Ice, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Ice, ElementTypes.Storm):
                break;

            case (ElementTypes.Ice, ElementTypes.Void):
                break;

            case (ElementTypes.Ice, ElementTypes.Life):
                break;

            case (ElementTypes.Ice, ElementTypes.Death):
                break;
            #endregion

            #region //Laser Interactions

            case (ElementTypes.Laser, ElementTypes.Fire):
                break;

            case (ElementTypes.Laser, ElementTypes.Water):
                break;

            case (ElementTypes.Laser, ElementTypes.Electric):
                break;

            case (ElementTypes.Laser, ElementTypes.Rock):
                break;

            case (ElementTypes.Laser, ElementTypes.Wind):
                break;

            case (ElementTypes.Laser, ElementTypes.Steam):
                break;

            case (ElementTypes.Laser, ElementTypes.Lava):
                break;

            case (ElementTypes.Laser, ElementTypes.Explosive):
                break;

            case (ElementTypes.Laser, ElementTypes.Plasma):
                break;

            case (ElementTypes.Laser, ElementTypes.Plants):
                break;

            case (ElementTypes.Laser, ElementTypes.Ice):
                break;

            case (ElementTypes.Laser, ElementTypes.Laser):
                break;

            case (ElementTypes.Laser, ElementTypes.Dust):
                break;

            case (ElementTypes.Laser, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Laser, ElementTypes.Storm):
                break;

            case (ElementTypes.Laser, ElementTypes.Void):
                break;

            case (ElementTypes.Laser, ElementTypes.Life):
                break;

            case (ElementTypes.Laser, ElementTypes.Death):
                break;
            #endregion

            #region //Dust Interactions

            case (ElementTypes.Dust, ElementTypes.Fire):
                break;

            case (ElementTypes.Dust, ElementTypes.Water):
                break;

            case (ElementTypes.Dust, ElementTypes.Electric):
                break;

            case (ElementTypes.Dust, ElementTypes.Rock):
                break;

            case (ElementTypes.Dust, ElementTypes.Wind):
                break;

            case (ElementTypes.Dust, ElementTypes.Steam):
                break;

            case (ElementTypes.Dust, ElementTypes.Lava):
                break;

            case (ElementTypes.Dust, ElementTypes.Explosive):
                break;

            case (ElementTypes.Dust, ElementTypes.Plasma):
                break;

            case (ElementTypes.Dust, ElementTypes.Plants):
                break;

            case (ElementTypes.Dust, ElementTypes.Ice):
                break;

            case (ElementTypes.Dust, ElementTypes.Laser):
                break;

            case (ElementTypes.Dust, ElementTypes.Dust):
                break;

            case (ElementTypes.Dust, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Dust, ElementTypes.Storm):
                break;

            case (ElementTypes.Dust, ElementTypes.Void):
                break;

            case (ElementTypes.Dust, ElementTypes.Life):
                break;

            case (ElementTypes.Dust, ElementTypes.Death):
                break;
            #endregion

            #region //Magnetism Interactions
            case (ElementTypes.Magnetism, ElementTypes.Fire):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Water):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Electric):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Rock):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Wind):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Steam):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Lava):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Explosive):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Plasma):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Plants):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Ice):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Laser):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Dust):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Storm):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Void):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Life):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Death):
                break;
            #endregion

            #region //Storm Interactions

            case (ElementTypes.Storm, ElementTypes.Fire):
                break;

            case (ElementTypes.Storm, ElementTypes.Water):
                break;

            case (ElementTypes.Storm, ElementTypes.Electric):
                break;

            case (ElementTypes.Storm, ElementTypes.Rock):
                break;

            case (ElementTypes.Storm, ElementTypes.Wind):
                break;

            case (ElementTypes.Storm, ElementTypes.Steam):
                break;

            case (ElementTypes.Storm, ElementTypes.Lava):
                break;

            case (ElementTypes.Storm, ElementTypes.Explosive):
                break;

            case (ElementTypes.Storm, ElementTypes.Plasma):
                break;

            case (ElementTypes.Storm, ElementTypes.Plants):
                break;

            case (ElementTypes.Storm, ElementTypes.Ice):
                break;

            case (ElementTypes.Storm, ElementTypes.Laser):
                break;

            case (ElementTypes.Storm, ElementTypes.Dust):
                break;

            case (ElementTypes.Storm, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Storm, ElementTypes.Storm):
                break;

            case (ElementTypes.Storm, ElementTypes.Void):
                break;

            case (ElementTypes.Storm, ElementTypes.Life):
                break;

            case (ElementTypes.Storm, ElementTypes.Death):
                break;
            #endregion

            #region //Void Interactions

            case (ElementTypes.Void, ElementTypes.Fire)://With all elements and subelemnts void interaction is dissapear them like a vacuum cleaner
                break;

            case (ElementTypes.Void, ElementTypes.Water):
                break;

            case (ElementTypes.Void, ElementTypes.Electric):
                break;

            case (ElementTypes.Void, ElementTypes.Rock):
                break;

            case (ElementTypes.Void, ElementTypes.Wind):
                break;

            case (ElementTypes.Void, ElementTypes.Steam):
                break;

            case (ElementTypes.Void, ElementTypes.Lava):
                break;

            case (ElementTypes.Void, ElementTypes.Explosive):
                break;

            case (ElementTypes.Void, ElementTypes.Plasma):
                break;

            case (ElementTypes.Void, ElementTypes.Plants):
                break;

            case (ElementTypes.Void, ElementTypes.Ice):
                break;

            case (ElementTypes.Void, ElementTypes.Laser):
                break;

            case (ElementTypes.Void, ElementTypes.Dust):
                break;

            case (ElementTypes.Void, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Void, ElementTypes.Storm):
                break;

            case (ElementTypes.Void, ElementTypes.Void):
                break;

            case (ElementTypes.Void, ElementTypes.Life):
                break;

            case (ElementTypes.Void, ElementTypes.Death):
                break;
            #endregion

            #region //Life Interactions

            case (ElementTypes.Life, ElementTypes.Fire)://With all elements and subelemnts life interaction is making them stronger/multiply/grow
                break;

            case (ElementTypes.Life, ElementTypes.Water):
                break;

            case (ElementTypes.Life, ElementTypes.Electric):
                break;

            case (ElementTypes.Life, ElementTypes.Rock):
                break;

            case (ElementTypes.Life, ElementTypes.Wind):
                break;

            case (ElementTypes.Life, ElementTypes.Steam):
                break;

            case (ElementTypes.Life, ElementTypes.Lava):
                break;

            case (ElementTypes.Life, ElementTypes.Explosive):
                break;

            case (ElementTypes.Life, ElementTypes.Plasma):
                break;

            case (ElementTypes.Life, ElementTypes.Plants):
                break;

            case (ElementTypes.Life, ElementTypes.Ice):
                break;

            case (ElementTypes.Life, ElementTypes.Laser):
                break;

            case (ElementTypes.Life, ElementTypes.Dust):
                break;

            case (ElementTypes.Life, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Life, ElementTypes.Storm):
                break;

            case (ElementTypes.Life, ElementTypes.Void):
                break;

            case (ElementTypes.Life, ElementTypes.Life):
                break;

            case (ElementTypes.Life, ElementTypes.Death):
                break;
            #endregion

            #region //Death Interactions

            case (ElementTypes.Death, ElementTypes.Fire)://With all elements and subelemnts death interaction is extinguish them
                break;

            case (ElementTypes.Death, ElementTypes.Water):
                break;

            case (ElementTypes.Death, ElementTypes.Electric):
                break;

            case (ElementTypes.Death, ElementTypes.Rock):
                break;

            case (ElementTypes.Death, ElementTypes.Wind):
                break;

            case (ElementTypes.Death, ElementTypes.Steam):
                break;

            case (ElementTypes.Death, ElementTypes.Lava):
                break;

            case (ElementTypes.Death, ElementTypes.Explosive):
                break;

            case (ElementTypes.Death, ElementTypes.Plasma):
                break;

            case (ElementTypes.Death, ElementTypes.Plants):
                break;

            case (ElementTypes.Death, ElementTypes.Ice):
                break;

            case (ElementTypes.Death, ElementTypes.Laser):
                break;

            case (ElementTypes.Death, ElementTypes.Dust):
                break;

            case (ElementTypes.Death, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Death, ElementTypes.Storm):
                break;

            case (ElementTypes.Death, ElementTypes.Void):
                break;

            case (ElementTypes.Death, ElementTypes.Life):
                break;

            case (ElementTypes.Death, ElementTypes.Death):
                break;
            #endregion

            default:
                Debug.Log("No interaction ¯\\_(ツ)_/¯");
                break;
        }
    }

    private void GenerateBiProducts(ElementTypes elementType1, ElementTypes elementType2)
    {
        switch (elementType1, elementType2)
        {
            #region //Fire interactions

            case (ElementTypes.Fire, ElementTypes.Fire)://Fire up
                break;

            case (ElementTypes.Fire, ElementTypes.Water)://Evaporarates water and produces steam
                break;

            case (ElementTypes.Fire, ElementTypes.Electric)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Rock)://Leaves a smoky decal, if enough heat is applied it turns into lava(maybe with tier 3 fire)
                break;

            case (ElementTypes.Fire, ElementTypes.Wind)://Fire tornado? Explosion?
                break;

            case (ElementTypes.Fire, ElementTypes.Steam)://Clears up the fog
                break;

            case (ElementTypes.Fire, ElementTypes.Lava)://If solid, mealts it up again
                break;

            case (ElementTypes.Fire, ElementTypes.Explosive)://if is a dormant explosive makes it explode, otherwise ?(explosions last seconds soo not much of a time window to interact with a live explosion)
                break;

            case (ElementTypes.Fire, ElementTypes.Plasma)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Plants)://Burns the plants and leaves charcoal maybe?
                break;

            case (ElementTypes.Fire, ElementTypes.Ice)://Melts the ice and produces water
                break;

            case (ElementTypes.Fire, ElementTypes.Laser)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Dust)://If is a cloud of dust/the dust is suspended in the air it generates an explosion otherwise turns it into a crystal/mirror surface?
                break;

            case (ElementTypes.Fire, ElementTypes.Magnetism)://Nothing, or could it make it stronger or weaker?, enable it or disable it?
                break;

            case (ElementTypes.Fire, ElementTypes.Storm)://Subdues the fire, sooo nothing really maybe the play the steam FX to convey the fire is being put out?
                break;

            case (ElementTypes.Fire, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Fire, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Water intereactions

            case (ElementTypes.Water, ElementTypes.Fire)://Puts ot the fire and evaporates producing steam
                break;

            case (ElementTypes.Water, ElementTypes.Water)://More water, adds water
                break;

            case (ElementTypes.Water, ElementTypes.Electric)://Electrifies the water
                break;

            case (ElementTypes.Water, ElementTypes.Rock)://Wet stone decal or wet shader?
                break;

            case (ElementTypes.Water, ElementTypes.Wind)://Water tornado, storm?
                break;

            case (ElementTypes.Water, ElementTypes.Steam)://Clears up the fog
                break;

            case (ElementTypes.Water, ElementTypes.Lava)://Cools and solidifies it
                break;

            case (ElementTypes.Water, ElementTypes.Explosive)://if is a dormant explosive ruins it so it no longer explodes even if it interact with fire, otherwise ?(explosions last seconds soo not much of a time window to interact with a live explosion)
                break;

            case (ElementTypes.Water, ElementTypes.Plasma)://Electrifies the water
                break;

            case (ElementTypes.Water, ElementTypes.Plants)://Makes the plants grow
                break;

            case (ElementTypes.Water, ElementTypes.Ice)://Freezes the water
                break;

            case (ElementTypes.Water, ElementTypes.Laser)://Electrifies the water
                break;

            case (ElementTypes.Water, ElementTypes.Dust)://Makes mud? or cleans the dust?
                break;

            case (ElementTypes.Water, ElementTypes.Magnetism)://Nothing
                break;

            case (ElementTypes.Water, ElementTypes.Storm)://Makes the storm stronger?
                break;

            case (ElementTypes.Water, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Water, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Water, ElementTypes.Death)://Produces poison?
                break;
            #endregion

            #region //Electric Interactions

            case (ElementTypes.Electric, ElementTypes.Fire)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Water)://Electrifies the water
                break;

            case (ElementTypes.Electric, ElementTypes.Electric)://Makes the electricity effect last longer or maybe recharges it?
                break;

            case (ElementTypes.Electric, ElementTypes.Rock)://Nothing, smoky decal?
                break;

            case (ElementTypes.Electric, ElementTypes.Wind)://Electric storm?
                break;

            case (ElementTypes.Electric, ElementTypes.Steam)://Nothing, clears the fog?
                break;

            case (ElementTypes.Electric, ElementTypes.Lava)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Explosive)://if is a dormant explosive makes it explode, otherwise ?(explosions last seconds soo not much of a time window to interact with a live explosion)
                break;

            case (ElementTypes.Electric, ElementTypes.Plasma)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Plants)://Burns the plants and leaves charcoal maybe?
                break;

            case (ElementTypes.Electric, ElementTypes.Ice)://Melts the ice and produces water
                break;

            case (ElementTypes.Electric, ElementTypes.Laser)://Nothing, changes its path/way?
                break;

            case (ElementTypes.Electric, ElementTypes.Dust)://Nothing, elctrostatic cloud of dust?
                break;

            case (ElementTypes.Electric, ElementTypes.Magnetism)://Could it make it stronger or weaker?, enable it or disable it?
                break;

            case (ElementTypes.Electric, ElementTypes.Storm)://Electric storm
                break;

            case (ElementTypes.Electric, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Electric, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Rock Interactions

            case (ElementTypes.Rock, ElementTypes.Fire)://Puts out the fire
                break;

            case (ElementTypes.Rock, ElementTypes.Water)://Nothing?, creates mud?
                break;

            case (ElementTypes.Rock, ElementTypes.Electric)://Nothing, grounds the electricity?
                break;

            case (ElementTypes.Rock, ElementTypes.Rock)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Wind)://Nothing, calms the wind
                break;

            case (ElementTypes.Rock, ElementTypes.Steam)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Lava)://Starts to melt but gradually
                break;

            case (ElementTypes.Rock, ElementTypes.Explosive)://Cracks/Breaks the rock
                break;

            case (ElementTypes.Rock, ElementTypes.Plasma)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Plants)://Destroys/Cuts/Smash plants
                break;

            case (ElementTypes.Rock, ElementTypes.Ice)://Crushs/Cracks/Breaks ice
                break;

            case (ElementTypes.Rock, ElementTypes.Laser)://Turns the rock into lava
                break;

            case (ElementTypes.Rock, ElementTypes.Dust)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Magnetism)://Nothing, insulates from magnetic fields? 
                break;

            case (ElementTypes.Rock, ElementTypes.Storm)://Calms/stops the storm
                break;

            case (ElementTypes.Rock, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Rock, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Wind Interactions
            case (ElementTypes.Wind, ElementTypes.Fire)://puts out the fire/kindles it(depends on the level maybe level 1 kindles it but level 3 puts the fire out)
                break;

            case (ElementTypes.Wind, ElementTypes.Water)://Nothing, moves the water?
                break;

            case (ElementTypes.Wind, ElementTypes.Electric)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Rock)://Nothing, if is strong enough maybe push/cuts rocks?
                break;

            case (ElementTypes.Wind, ElementTypes.Wind)://More wind
                break;

            case (ElementTypes.Wind, ElementTypes.Steam)://Blows away the fog
                break;

            case (ElementTypes.Wind, ElementTypes.Lava)://Cools it down but significantly slower tahan water or ice
                break;

            case (ElementTypes.Wind, ElementTypes.Explosive)://If its dormant nothing otherwise maybe blow the fumes from the explosion?
                break;

            case (ElementTypes.Wind, ElementTypes.Plasma)://Nothing, maybe electrify the air?
                break;

            case (ElementTypes.Wind, ElementTypes.Plants)://Moves the plants, maybe if ii is strong enough blows away the plants like on a wall or the ground
                break;

            case (ElementTypes.Wind, ElementTypes.Ice)://Nothing, if is strong enough maybe push/cuts ice blocks
                break;

            case (ElementTypes.Wind, ElementTypes.Laser)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Dust)://Blows off the dust
                break;

            case (ElementTypes.Wind, ElementTypes.Magnetism)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Storm)://Makes it stronger, if strong enough maybe blows it or moves it?
                break;

            case (ElementTypes.Wind, ElementTypes.Void)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Life)://Nothing
                break;

            case (ElementTypes.Wind, ElementTypes.Death)://Nothing
                break;
            #endregion

            #region //Steam Interactions

            case (ElementTypes.Steam, ElementTypes.Fire):
                break;

            case (ElementTypes.Steam, ElementTypes.Water):
                break;

            case (ElementTypes.Steam, ElementTypes.Electric):
                break;

            case (ElementTypes.Steam, ElementTypes.Rock):
                break;

            case (ElementTypes.Steam, ElementTypes.Wind):
                break;

            case (ElementTypes.Steam, ElementTypes.Steam):
                break;

            case (ElementTypes.Steam, ElementTypes.Lava):
                break;

            case (ElementTypes.Steam, ElementTypes.Explosive):
                break;

            case (ElementTypes.Steam, ElementTypes.Plasma):
                break;

            case (ElementTypes.Steam, ElementTypes.Plants):
                break;

            case (ElementTypes.Steam, ElementTypes.Ice):
                break;

            case (ElementTypes.Steam, ElementTypes.Laser):
                break;

            case (ElementTypes.Steam, ElementTypes.Dust):
                break;

            case (ElementTypes.Steam, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Steam, ElementTypes.Storm):
                break;

            case (ElementTypes.Steam, ElementTypes.Void):
                break;

            case (ElementTypes.Steam, ElementTypes.Life):
                break;

            case (ElementTypes.Steam, ElementTypes.Death):
                break;
            #endregion

            #region //Lava Interactions

            case (ElementTypes.Lava, ElementTypes.Fire):
                break;

            case (ElementTypes.Lava, ElementTypes.Water):
                break;

            case (ElementTypes.Lava, ElementTypes.Electric):
                break;

            case (ElementTypes.Lava, ElementTypes.Rock):
                break;

            case (ElementTypes.Lava, ElementTypes.Wind):
                break;

            case (ElementTypes.Lava, ElementTypes.Steam):
                break;

            case (ElementTypes.Lava, ElementTypes.Lava):
                break;

            case (ElementTypes.Lava, ElementTypes.Explosive):
                break;

            case (ElementTypes.Lava, ElementTypes.Plasma):
                break;

            case (ElementTypes.Lava, ElementTypes.Plants):
                break;

            case (ElementTypes.Lava, ElementTypes.Ice):
                break;

            case (ElementTypes.Lava, ElementTypes.Laser):
                break;

            case (ElementTypes.Lava, ElementTypes.Dust):
                break;

            case (ElementTypes.Lava, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Lava, ElementTypes.Storm):
                break;

            case (ElementTypes.Lava, ElementTypes.Void):
                break;

            case (ElementTypes.Lava, ElementTypes.Life):
                break;

            case (ElementTypes.Lava, ElementTypes.Death):
                break;
            #endregion

            #region //Explosive Interactions

            case (ElementTypes.Explosive, ElementTypes.Fire):
                break;

            case (ElementTypes.Explosive, ElementTypes.Water):
                break;

            case (ElementTypes.Explosive, ElementTypes.Electric):
                break;

            case (ElementTypes.Explosive, ElementTypes.Rock):
                break;

            case (ElementTypes.Explosive, ElementTypes.Wind):
                break;

            case (ElementTypes.Explosive, ElementTypes.Steam):
                break;

            case (ElementTypes.Explosive, ElementTypes.Lava):
                break;

            case (ElementTypes.Explosive, ElementTypes.Explosive):
                break;

            case (ElementTypes.Explosive, ElementTypes.Plasma):
                break;

            case (ElementTypes.Explosive, ElementTypes.Plants):
                break;

            case (ElementTypes.Explosive, ElementTypes.Ice):
                break;

            case (ElementTypes.Explosive, ElementTypes.Laser):
                break;

            case (ElementTypes.Explosive, ElementTypes.Dust):
                break;

            case (ElementTypes.Explosive, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Explosive, ElementTypes.Storm):
                break;

            case (ElementTypes.Explosive, ElementTypes.Void):
                break;

            case (ElementTypes.Explosive, ElementTypes.Life):
                break;

            case (ElementTypes.Explosive, ElementTypes.Death):
                break;
            #endregion

            #region //Plasma Interactions

            case (ElementTypes.Plasma, ElementTypes.Fire):
                break;

            case (ElementTypes.Plasma, ElementTypes.Water):
                break;

            case (ElementTypes.Plasma, ElementTypes.Electric):
                break;

            case (ElementTypes.Plasma, ElementTypes.Rock):
                break;

            case (ElementTypes.Plasma, ElementTypes.Wind):
                break;

            case (ElementTypes.Plasma, ElementTypes.Steam):
                break;

            case (ElementTypes.Plasma, ElementTypes.Lava):
                break;

            case (ElementTypes.Plasma, ElementTypes.Explosive):
                break;

            case (ElementTypes.Plasma, ElementTypes.Plasma):
                break;

            case (ElementTypes.Plasma, ElementTypes.Plants):
                break;

            case (ElementTypes.Plasma, ElementTypes.Ice):
                break;

            case (ElementTypes.Plasma, ElementTypes.Laser):
                break;

            case (ElementTypes.Plasma, ElementTypes.Dust):
                break;

            case (ElementTypes.Plasma, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Plasma, ElementTypes.Storm):
                break;

            case (ElementTypes.Plasma, ElementTypes.Void):
                break;

            case (ElementTypes.Plasma, ElementTypes.Life):
                break;

            case (ElementTypes.Plasma, ElementTypes.Death):
                break;
            #endregion

            #region //Plants Interactions

            case (ElementTypes.Plants, ElementTypes.Fire):
                break;

            case (ElementTypes.Plants, ElementTypes.Water):
                break;

            case (ElementTypes.Plants, ElementTypes.Electric):
                break;

            case (ElementTypes.Plants, ElementTypes.Rock):
                break;

            case (ElementTypes.Plants, ElementTypes.Wind):
                break;

            case (ElementTypes.Plants, ElementTypes.Steam):
                break;

            case (ElementTypes.Plants, ElementTypes.Lava):
                break;

            case (ElementTypes.Plants, ElementTypes.Explosive):
                break;

            case (ElementTypes.Plants, ElementTypes.Plasma):
                break;

            case (ElementTypes.Plants, ElementTypes.Plants):
                break;

            case (ElementTypes.Plants, ElementTypes.Ice):
                break;

            case (ElementTypes.Plants, ElementTypes.Laser):
                break;

            case (ElementTypes.Plants, ElementTypes.Dust):
                break;

            case (ElementTypes.Plants, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Plants, ElementTypes.Storm):
                break;

            case (ElementTypes.Plants, ElementTypes.Void):
                break;

            case (ElementTypes.Plants, ElementTypes.Life):
                break;

            case (ElementTypes.Plants, ElementTypes.Death):
                break;
            #endregion

            #region //Ice Interactions

            case (ElementTypes.Ice, ElementTypes.Fire):
                break;

            case (ElementTypes.Ice, ElementTypes.Water):
                break;

            case (ElementTypes.Ice, ElementTypes.Electric):
                break;

            case (ElementTypes.Ice, ElementTypes.Rock):
                break;

            case (ElementTypes.Ice, ElementTypes.Wind):
                break;

            case (ElementTypes.Ice, ElementTypes.Steam):
                break;

            case (ElementTypes.Ice, ElementTypes.Lava):
                break;

            case (ElementTypes.Ice, ElementTypes.Explosive):
                break;

            case (ElementTypes.Ice, ElementTypes.Plasma):
                break;

            case (ElementTypes.Ice, ElementTypes.Plants):
                break;

            case (ElementTypes.Ice, ElementTypes.Ice):
                break;

            case (ElementTypes.Ice, ElementTypes.Laser):
                break;

            case (ElementTypes.Ice, ElementTypes.Dust):
                break;

            case (ElementTypes.Ice, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Ice, ElementTypes.Storm):
                break;

            case (ElementTypes.Ice, ElementTypes.Void):
                break;

            case (ElementTypes.Ice, ElementTypes.Life):
                break;

            case (ElementTypes.Ice, ElementTypes.Death):
                break;
            #endregion

            #region //Laser Interactions

            case (ElementTypes.Laser, ElementTypes.Fire):
                break;

            case (ElementTypes.Laser, ElementTypes.Water):
                break;

            case (ElementTypes.Laser, ElementTypes.Electric):
                break;

            case (ElementTypes.Laser, ElementTypes.Rock):
                break;

            case (ElementTypes.Laser, ElementTypes.Wind):
                break;

            case (ElementTypes.Laser, ElementTypes.Steam):
                break;

            case (ElementTypes.Laser, ElementTypes.Lava):
                break;

            case (ElementTypes.Laser, ElementTypes.Explosive):
                break;

            case (ElementTypes.Laser, ElementTypes.Plasma):
                break;

            case (ElementTypes.Laser, ElementTypes.Plants):
                break;

            case (ElementTypes.Laser, ElementTypes.Ice):
                break;

            case (ElementTypes.Laser, ElementTypes.Laser):
                break;

            case (ElementTypes.Laser, ElementTypes.Dust):
                break;

            case (ElementTypes.Laser, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Laser, ElementTypes.Storm):
                break;

            case (ElementTypes.Laser, ElementTypes.Void):
                break;

            case (ElementTypes.Laser, ElementTypes.Life):
                break;

            case (ElementTypes.Laser, ElementTypes.Death):
                break;
            #endregion

            #region //Dust Interactions

            case (ElementTypes.Dust, ElementTypes.Fire):
                break;

            case (ElementTypes.Dust, ElementTypes.Water):
                break;

            case (ElementTypes.Dust, ElementTypes.Electric):
                break;

            case (ElementTypes.Dust, ElementTypes.Rock):
                break;

            case (ElementTypes.Dust, ElementTypes.Wind):
                break;

            case (ElementTypes.Dust, ElementTypes.Steam):
                break;

            case (ElementTypes.Dust, ElementTypes.Lava):
                break;

            case (ElementTypes.Dust, ElementTypes.Explosive):
                break;

            case (ElementTypes.Dust, ElementTypes.Plasma):
                break;

            case (ElementTypes.Dust, ElementTypes.Plants):
                break;

            case (ElementTypes.Dust, ElementTypes.Ice):
                break;

            case (ElementTypes.Dust, ElementTypes.Laser):
                break;

            case (ElementTypes.Dust, ElementTypes.Dust):
                break;

            case (ElementTypes.Dust, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Dust, ElementTypes.Storm):
                break;

            case (ElementTypes.Dust, ElementTypes.Void):
                break;

            case (ElementTypes.Dust, ElementTypes.Life):
                break;

            case (ElementTypes.Dust, ElementTypes.Death):
                break;
            #endregion

            #region //Magnetism Interactions
            case (ElementTypes.Magnetism, ElementTypes.Fire):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Water):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Electric):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Rock):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Wind):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Steam):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Lava):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Explosive):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Plasma):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Plants):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Ice):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Laser):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Dust):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Storm):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Void):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Life):
                break;

            case (ElementTypes.Magnetism, ElementTypes.Death):
                break;
            #endregion

            #region //Storm Interactions

            case (ElementTypes.Storm, ElementTypes.Fire):
                break;

            case (ElementTypes.Storm, ElementTypes.Water):
                break;

            case (ElementTypes.Storm, ElementTypes.Electric):
                break;

            case (ElementTypes.Storm, ElementTypes.Rock):
                break;

            case (ElementTypes.Storm, ElementTypes.Wind):
                break;

            case (ElementTypes.Storm, ElementTypes.Steam):
                break;

            case (ElementTypes.Storm, ElementTypes.Lava):
                break;

            case (ElementTypes.Storm, ElementTypes.Explosive):
                break;

            case (ElementTypes.Storm, ElementTypes.Plasma):
                break;

            case (ElementTypes.Storm, ElementTypes.Plants):
                break;

            case (ElementTypes.Storm, ElementTypes.Ice):
                break;

            case (ElementTypes.Storm, ElementTypes.Laser):
                break;

            case (ElementTypes.Storm, ElementTypes.Dust):
                break;

            case (ElementTypes.Storm, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Storm, ElementTypes.Storm):
                break;

            case (ElementTypes.Storm, ElementTypes.Void):
                break;

            case (ElementTypes.Storm, ElementTypes.Life):
                break;

            case (ElementTypes.Storm, ElementTypes.Death):
                break;
            #endregion

            #region //Void Interactions

            case (ElementTypes.Void, ElementTypes.Fire)://With all elements and subelemnts void interaction is dissapear them like a vacuum cleaner
                break;

            case (ElementTypes.Void, ElementTypes.Water):
                break;

            case (ElementTypes.Void, ElementTypes.Electric):
                break;

            case (ElementTypes.Void, ElementTypes.Rock):
                break;

            case (ElementTypes.Void, ElementTypes.Wind):
                break;

            case (ElementTypes.Void, ElementTypes.Steam):
                break;

            case (ElementTypes.Void, ElementTypes.Lava):
                break;

            case (ElementTypes.Void, ElementTypes.Explosive):
                break;

            case (ElementTypes.Void, ElementTypes.Plasma):
                break;

            case (ElementTypes.Void, ElementTypes.Plants):
                break;

            case (ElementTypes.Void, ElementTypes.Ice):
                break;

            case (ElementTypes.Void, ElementTypes.Laser):
                break;

            case (ElementTypes.Void, ElementTypes.Dust):
                break;

            case (ElementTypes.Void, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Void, ElementTypes.Storm):
                break;

            case (ElementTypes.Void, ElementTypes.Void):
                break;

            case (ElementTypes.Void, ElementTypes.Life):
                break;

            case (ElementTypes.Void, ElementTypes.Death):
                break;
            #endregion

            #region //Life Interactions

            case (ElementTypes.Life, ElementTypes.Fire)://With all elements and subelemnts life interaction is making them stronger/multiply/grow
                break;

            case (ElementTypes.Life, ElementTypes.Water):
                break;

            case (ElementTypes.Life, ElementTypes.Electric):
                break;

            case (ElementTypes.Life, ElementTypes.Rock):
                break;

            case (ElementTypes.Life, ElementTypes.Wind):
                break;

            case (ElementTypes.Life, ElementTypes.Steam):
                break;

            case (ElementTypes.Life, ElementTypes.Lava):
                break;

            case (ElementTypes.Life, ElementTypes.Explosive):
                break;

            case (ElementTypes.Life, ElementTypes.Plasma):
                break;

            case (ElementTypes.Life, ElementTypes.Plants):
                break;

            case (ElementTypes.Life, ElementTypes.Ice):
                break;

            case (ElementTypes.Life, ElementTypes.Laser):
                break;

            case (ElementTypes.Life, ElementTypes.Dust):
                break;

            case (ElementTypes.Life, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Life, ElementTypes.Storm):
                break;

            case (ElementTypes.Life, ElementTypes.Void):
                break;

            case (ElementTypes.Life, ElementTypes.Life):
                break;

            case (ElementTypes.Life, ElementTypes.Death):
                break;
            #endregion

            #region //Death Interactions

            case (ElementTypes.Death, ElementTypes.Fire)://With all elements and subelemnts death interaction is extinguish them
                break;

            case (ElementTypes.Death, ElementTypes.Water):
                break;

            case (ElementTypes.Death, ElementTypes.Electric):
                break;

            case (ElementTypes.Death, ElementTypes.Rock):
                break;

            case (ElementTypes.Death, ElementTypes.Wind):
                break;

            case (ElementTypes.Death, ElementTypes.Steam):
                break;

            case (ElementTypes.Death, ElementTypes.Lava):
                break;

            case (ElementTypes.Death, ElementTypes.Explosive):
                break;

            case (ElementTypes.Death, ElementTypes.Plasma):
                break;

            case (ElementTypes.Death, ElementTypes.Plants):
                break;

            case (ElementTypes.Death, ElementTypes.Ice):
                break;

            case (ElementTypes.Death, ElementTypes.Laser):
                break;

            case (ElementTypes.Death, ElementTypes.Dust):
                break;

            case (ElementTypes.Death, ElementTypes.Magnetism):
                break;

            case (ElementTypes.Death, ElementTypes.Storm):
                break;

            case (ElementTypes.Death, ElementTypes.Void):
                break;

            case (ElementTypes.Death, ElementTypes.Life):
                break;

            case (ElementTypes.Death, ElementTypes.Death):
                break;
            #endregion

            default:
                Debug.Log("No interaction ¯\\_(ツ)_/¯");
                break;
        }
    }

    private void DrainGemEnergySpentInInteraction(Item item, float energySpent)
    {
        switch (item.itemType)
        {

            case ItemType.Gem:
                break;

            case ItemType.Weapon:
                WeaponItem weaponItem = item as WeaponItem;
                List<GemItem> weaponGems = new List<GemItem>();
                for (int i = 0; i < weaponItem.gemSockets.Length; i++)
                {
                    if (weaponItem.gemSockets[i] != null)
                    {
                        weaponGems.Add(weaponItem.gemSockets[i]);
                    }
                }
                switch (weaponGems.Count)
                {
                    case 0:
                        //Impossible it must have a gem to interact
                        return;
                    case 1:
                        weaponGems[0].currentEnergy -= energySpent;
                        break;
                    case 2:
                        weaponGems[0].currentEnergy -= energySpent / 2;
                        weaponGems[1].currentEnergy -= energySpent / 2;
                        break;
                    case 3:
                        weaponGems[0].currentEnergy -= energySpent / 3;
                        weaponGems[1].currentEnergy -= energySpent / 3;
                        weaponGems[2].currentEnergy -= energySpent / 3;
                        break;
                    default:
                        break;
                }

                break;

            default:
                break;
        }
    }

    private void DrainGemEnergySpentInInteraction(GemsInObjectsManager gemsInObjectsManager, float energySpent)
    {
        List<GemItem> itemGems = gemsInObjectsManager.GetEquipedGems();
        switch (itemGems.Count)
        {
            case 0:
                //Impossible it must have a gem to interact
                return;
            case 1:
                itemGems[0].currentEnergy -= energySpent;
                break;
            case 2:
                itemGems[0].currentEnergy -= energySpent / 2;
                itemGems[1].currentEnergy -= energySpent / 2;
                break;
            case 3:
                itemGems[0].currentEnergy -= energySpent / 3;
                itemGems[1].currentEnergy -= energySpent / 3;
                itemGems[2].currentEnergy -= energySpent / 3;
                break;
            default:
                break;
        }
    }

    public void SetElementType(ElementTypes elementType)
    {
        _elementType = elementType;
    }

    public ElementTypes GetElementType()
    {
        return _elementType;
    }

    public void SetElementLevel(ElementLevel elementLevel)
    {
        _elementLevel = elementLevel;
    }

    public ElementLevel GetElementLevel()
    {
        return _elementLevel;
    }
}
