using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogsData
{
    // ' ' separates words (no pause).
    // '. ' Pause
    // <P> Pause
    public static Dictionary<string, List<List<string>>> DialogTree = new Dictionary<string, List<List<string>>>()
    {
        #region Podarge
        { "Podarge|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Hmm yes? What is it?",
                    "Well... Don't you know your master started a realm war against-",
                    "Wait What!! Oh I guess I was asleep during this morning meeting...",
                    "...",
                    "as... as usual... ",
                    "...",
                    "Ok ok let's pretend I was actually aware of that from the beggining!",
                    "Well... I guess you were, weren't you?",
                    "That's the spirit!"
                }
            }
        },
        { "Podarge|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Oh hi Edam!",
                    "Podarge... Can you explain to me what are you doing here?",
                    "Did I... Did I miss something?",
                    "War between realms? Escaping hell? Kicking angels asses? Does it rings a bell?",
                    "...",
                    "...",
                    "...",
                    "Asleep during the morning meeting again I suppose?",
                    "Edam...",
                    "[ Sigh ] \nJust... Just follow us and try to keep the pace. Please."
                }
            }
        },
        { "Podarge|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Oh hi my queen!",
                    "Podarge!! Chilling here like nothing ever happened?",
                    "Oh my Stan, did I miss something again?",
                    "I still don't know how Edam keeps his calm around you...",
                    "Well, the lust and depravity helps I guess!",
                    "...",
                    "...",
                    "Will you help us getting out of hell or what?",
                    "Oh! That's what it is about?",
                    "[ Sigh ] \nJust... Just follow us and try to keep the pace. Please."
                }
            }
        },
        #endregion
        #region Belias
        { "Belias|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Who dares disturb me during my eternal suffering?",
                    "Well... Wouldn't you want to do something else than... Suffering?",
                    "Just doing my chores, I know my place. Now begone! Before I loose my patience.",
                    "Even if I tell you that your mistress Ivy is sending me here to get you?",
                    "Mistress!! She talked about me? Say no more! I'm coming with you little being!",
                    "Little... being... ?"
                }
            }
        },
        { "Belias|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Mistress!! Oh mistress! Your presence here honors me!",
                    "Yeah yeah whatever. Come. We need you. Getting out of hell might be tricky without you!",
                    "Getting... out of hell?",
                    "Are you questioning my orders now?",
                    "Oh!! No I'm not, Mistress!! But, what is your father's opinion on this matter?",
                    "Belias. My patience has its limits.",
                    "Yes Mistress!!"
                }
            }
        },
        #endregion
        #region Floppyredoux
        { "Floppyredoux|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Who dares disturbing my precious time?",
                    "Oh, sorry I didn't know... My mistress sent me here to find you.",
                    "Your mistress?",
                    "Ivy, first born of Lust, rings a bell?",
                    "Ohh miss Ivy! Same potion as last time?",
                    "Well I'm not here for a potion I'm afraid.",
                    "Oh!! Did her \"horns issue\" got finally fixed?",
                    "Her... Horns issue?",
                    "Yeah... I should respect that patient privacy thing..."
                }
            }
        },
        { "Floppyredoux|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Mister Edam! Glad to see you back! And in the flesh this time! How is your sister's \"horns issue\" doing?",
                    "Hi Flo! It grew back thankfully! But I'm not here for this matter.",
                    "What brings your hellish ass then?",
                    "Do you recall of the guys from up there? Who banished your from cities and prohibited your witchcraft?",
                    "How could I forget...",
                    "Want your revenge?",
                    "Say no more!"
                }
            }
        },
        { "Floppyredoux|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Miss Ivy? In the flesh? How is your \"horns issue\" doing?",
                    "Hi witch... Your potions worked, my horn grew back...",
                    "Oh good to hear! You really should be more careful with your sex life dear!",
                    "Yeah yeah... But I'm not here to gossip about which demon I have a crush on this week...",
                    "What brings your hellish ass then?",
                    "Do you recall of the guys from up there? Who banished your from cities and prohibited your witchcraft?",
                    "How could I forget...",
                    "Want your revenge?",
                    "Say no more!"
                }
            }
        },
        { "Floppyredoux|Podarge",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Podarge my girl! How long has it been since I saw you?",
                    "Wassup Floflo! Way too long!",
                    "Here to get Miss Ivy's potion?",
                    "She told me her horn grew back! She might not need it anymore!!",
                    "Oh neat! To what do I owe the pleasure then?",
                    "She now wants every help she can get to kick some angels asses...",
                    "Ugh... Still bossy I see..."
                }
            }
        },
        #endregion
        #region Sir Vixid
        { "Sir Vixid|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Begone before I lose my temper.",
                    "Wow I think we went on the wrong foot here. I'm pretty sure we share the same faith here!",
                    "I'm listening.",
                    "Well you see, I'm sent here to you by my mistress Ivy, first born of the lust.",
                    "I'll be damned, are you the prophet we've been waiting for?",
                    "I wouldn't be that arrogant..."
                }
            }
        },
        { "Sir Vixid|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Finally, an answer to my prayers! Master Edam, is that you?",
                    "Oh, an adept of the temple!",
                    "My lord, I am here before you in quest for absolution, for all that is unholy!",
                    "What is your name fidel?",
                    "I am Sir Vixid, an adherent of the temple of evil.",
                    "Well then, Sir, what about a promotion within your rank?"
                }
            }
        },
        { "Sir Vixid|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Finally, an answer to my prayers!",
                    "Sigh... What kind of lunatic are you?",
                    "I, Sir Vixid, started an unholy crusade in your name decades ago, Ivy, first born of the lust!",
                    "Oh... You are growing on me Vixid. What are you doing later?"
                }
            }
        },
        { "Sir Vixid|Podarge",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Begone heretic, before I lose my temper.",
                    "Can't you see I'm a harpy? Am I not a creature you should worship in your dogma?",
                    "Aren't you an angel?",
                    "I beg your pardon???",
                    "Sorry miss, I've... I've seen many horrible things..."
                }
            }
        },
        { "Sir Vixid|Belias",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Finally, an answer to my prayers!",
                    "I see you are an adept of the temple!",
                    "Demon, I am here before you in quest for absolution, for all that is unholy!",
                    "What is your name fidel?",
                    "I am Sir Vixid, an adherent of the temple of evil.",
                    "Well then, Sir, what about a promotion within your rank?"
                }
            }
        },
        #endregion
        #region Cereza
        { "Cereza|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Oh look at you! What a cutie!!",
                    "...",
                    "What? Never seen a nun on high heels before?",
                    "Those are high indeed! And what about the mini skirt?",
                    "Stripper high!! And well, listen, I've had a revelation a couple days ago. I got sick of this chasteness...",
                    "How did the church saw your transition?",
                    "Oh screw them... I am now resolved to do anything to get rid of their grip!",
                    "Hehe, I think I can get you a cause to fight for!"
                }
            }
        },
        #endregion
        #region Tony
        { "Tony|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Hi, I'll take one \"white russian\", stirred not shaken.",
                    "...",
                    "Please?",
                    "Who do you think I am?",
                    "Not a good bartender that's for sure. Now quick, go get my drink!",
                    "Sign... Can't believe we need help that desperately to hire dudes like you...",
                    "Dudes like me? To hire me? Are you expecting me to work for you?",
                    "If I get you your drink, will you help us kicking some asses?",
                    "Are you expecting me to say no to a free drink? That's a deal!!",
                    "One \"white russian\" incoming!"
                }
            }
        },
        #endregion

        #region Harpy
        { "Harpy|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Masster???",
                    "You traitor!!"
                }
            }
        },
        { "Harpy|Podarge",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "You??",
                    "This is awkward..."
                }
            }
        },
        #endregion
        #region PHILL
        { "PHILL|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Hi!! Name's Phill!",
                    "Hi... Phill?",
                    "Time is dead, and meaning has no meaning!!",
                    "Excuse me?",
                    "Existence is upside down and I reign supreme!!"
                }
            }
        },
        #endregion
        #region Boom Slayer
        { "Boom Slayer|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "RIP!!!",
                    "RIP???",
                    "... AND TEAR!!!",
                    "You done?"
                }
            }
        },
        #endregion
        #region Shop Keeper
        { "Shop Keeper|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Geez! I was just about to do the thing!!",
                    "The thing?",
                    "Yes... The THING!!!"
                }
            }
        },
        #endregion
        #region Impostor
        { "Impostor|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "It wasn't me I swear!! I was in admin the whole time!!",
                    "What are you talking about?",
                    "Well, your loss then!!"
                }
            }
        },
        #endregion
        #region Baphomeh
        { "Baphomeh",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Argghh..."
                }
            }
        },
        { "Baphomeh|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Go back where you come from vermin! Those gates have remained sealed for centuries and I am here to keep it that way.",
                    "Vermin?"
                }
            }
        },
        { "Baphomeh|Earth",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Go back where you come from human! Enjoy your stay in this place as long as your corpse can handle it."
                }
            }
        },
        { "Baphomeh|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Greeting mistress. You must know that I'm not here, standing before you, by choice. Please forgive me...",
                    "You do not have to worry my dear Baphomeh. You have your orders, and I have my reasons. I respect your loyalty.",
                    "Let me then show you that I deserve your praise."
                },
                new List<string>()
                {
                    "Greeting mistress. You must know that I'm not here, standing before you, by choice. Please forgive me...",
                    "Yeah yeah I know, you already said it before...",
                    "I... I beg your pardon?",
                    "Bring it on Baphomeh."
                },
                new List<string>()
                {
                    "Mistress, you must know that-",
                    "You are not here, standing before me, by choice. Yeah I know I know... Let's do it!"
                }
            }
        },
        { "Baphomeh|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "My lord. Orders are orders. Nothing personal.",
                    "Be a good pawn Baphomeh, and show me what you got!",
                    "[ Snort ]"
                }
            }
        },
        { "Baphomeh|Belias",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Brother. Did Mistress Ivy send you?",
                    "She did. Will it be a problem?",
                    "I know you seek vengeance, and I won't stop you trying to defeat me once again. But you already know how it always ends.",
                    "This is about to change big brother. Now come, I'll show you what my exile made of me!!"
                },
                new List<string>()
                {
                    "Brother. Did Mistress Ivy send you?",
                    "Shut up, you. I'm here to continue our fight.",
                    "Continue? What do you mean??",
                    "That I finally have an opportunity to kick your hairy butt over and over.",
                    "Wait... this seems familiar...",
                    "You bet it does!!"
                }
            }
        },
        #endregion
        #region Devil's Advocate
        { "Devil's Advocate",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "What... No! Impossible!!"
                }
            }
        },
        { "Devil's Advocate|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Have you lost your mind??? Do you have any idea how many souls I have to judge instead of cleaning your mess?"
                }
            }
        },
        { "Devil's Advocate|Earth",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "A human here? Wait a minute, I cannot find your soul in the registries... How did you even end up here???",
                    "I've been told some amulet brought me here.",
                    "Oh... The evil twins... What have they done again..."
                }
            }
        },
        { "Devil's Advocate|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Sigh... What are you even doing here milady? I have work to do, and you are wasting my precious time...",
                    "Well, I won't bother you any longer in that case. Just let me through and call it a deal !",
                    "Do not mistake yourself. I know my priorities, I just wish you wouldn't be one..."
                },
                new List<string>()
                {
                    "Wait, something's weird... What is happening milady?",
                    "Wondering about my deeds my dear ?",
                    "It just feels like... I'm already bored of it even if it hasn't begun... Have you done something against the time continuum???",
                    "Hehehe!!!"
                },
                new List<string>()
                {
                    "Hmm... Why am I not surprised to see you here milady?",
                    "Might be linked to the fact that we have already done it several times maybe!!",
                    "Wait... No!!! The amulet ?",
                    "Haha, seeing you this surprised each time is so pleasing!!"
                }
            }
        },
        { "Devil's Advocate|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Look, the turbulent twin... How am I not even surprised to see you here?",
                    "You always had the kind words didn't you ?",
                    "Should I behave another way perhaps ?",
                    "No no keep it that way, it reinforces my will to erase you!!"
                },
                new List<string>()
                {
                    "Ugh... You again? Wait... Why again... What's happening Edam?",
                    "Come on, you're the one who should know it before anyone else!",
                    "What have you twins done again... Don't tell me you stole the time amulet!!",
                    "Haha, now you begin to be worth your reputation!"
                },
                new List<string>()
                {
                    "Edam... Seeing you here seems like a deja vu! I guess I've already failed before didn't I?",
                    "Haha! You've been a pleasing opponent so far don't worry.",
                    "I see... I hate this smile of yours. Let me put an end to it!!"
                }
            }
        },
        { "Devil's Advocate|Podarge",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Are you kidding me?",
                    "?",
                    "Of all the twins' fidels, they sent you? against me??",
                    "Hmm... Who are you again?",
                    "What?",
                    "What?",
                    "[ intelligible scream ]"
                }
            }
        },
        #endregion
        #region Stan
        { "Stan",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Haha, let there be darkness then..."
                }
            }
        },
        { "Stan|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "So... you are the one making this mess. I hope you know what you're doing, and what are the consequences!!"
                }
            }
        },
        { "Stan|Earth",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Is this some kind of joke??",
                    "Why would it be?",
                    "A mere human, trying to get out of hell. My children are really taking this matter too lightly..."
                }
            }
        },
        { "Stan|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Daughter. Seeing you here fills my heart with sadness...",
                    "Haha don't even try it. I know I have no place in your heart!",
                    "... \nWhat are you doing here Ivy?",
                    "Don't you see what's happening father? All those lost souls? Hell being incapable of doing its work, the humans not believing in us anymore?",
                    "What is your point?",
                    "You've grown weak! Hell's strength has grown weak, you know we need more fidels, we are losing this war against heaven!",
                    "A war?? Have you lost your mind? Our realms need balance, not war.",
                    "What balance are you talking about? Heaven is mocking us, they've won this \"balance\" from the beginning. I'm here to put an end to this fake peace!!!",
                    "I won't let you do it Ivy. There are rules!",
                    "Screw your rules and face me. Father!!"
                },
                new List<string>()
                {
                    "Daughter. Here for another round?",
                    "Wait, how do you know we've done it before?",
                    "Do you really think I wouldn't notice the effects of the amulet. I'm the overlord my dear.",
                    "So... You acknowledge what we are trying to pull off?",
                    "Not at all, but I'm curious to see the end of it! Now, en garde, daughter."
                },
                new List<string>()
                {
                    "Ah Ivy, I've been waiting for you.",
                    "Don't tell me that like it's nothing...",
                    "I'm getting used to it by now. Now make your father proud and show him your progress."
                },
                new List<string>()
                {
                    "Daughter.",
                    "Father."
                }
            }
        },
        { "Stan|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Edam. Helping your sister and her futile plan?",
                    "You know it dad. But I'm not only doing this for her. I get my part in this.",
                    "And what is it if I may ask?",
                    "I finally have a good reason to kick your heavenly ass!!",
                    "Still feeling bad being an angel offspring I see...",
                    "Don't... Don't say it! It's not about me it's about you. Now bring it on!!"
                },
                new List<string>()
                {
                    "Son. Still rejecting your nature?",
                    "I came here to- Wait!! You remember our fight?",
                    "Do you think a simple trick like this amulet would let me unaware of what's happening?",
                    "You call that a simple trick? It seems to be well enough for all the simple minded we fight each time!",
                    "Do not underestimate your overlord!!"
                },
                new List<string>()
                {
                    "Ah Edam, I've been waiting for you.",
                    "I'll make you regret the wait."
                },
                new List<string>()
                {
                    "Son.",
                    "Shut up..."
                }
            }
        },
        #endregion

        #region Monk
        { "Monk|Tony",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Patriarch Anthony?",
                    "Just Tony. Please.",
                    "But... But... What happened?",
                    "Alcohol, monk. Alcohol happened..."
                }
            }
        },
        #endregion
        #region Nun
        { "Nun|Cereza",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "...",
                    "...",
                    "What in the name of god... are you really a nun?",
                    "Haha, do you like what you see?"
                }
            }
        },
        #endregion
        #region Crusader
        { "Crusader|Sir Vixid",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Oh! A fierce opponent of the Army of The Night!",
                    "Oh... A scumbag of the Church...",
                    "Can't you have at least some dignity?",
                    "I'm not here for dignity. Now come taste my spear."
                }
            }
        },
        #endregion
        #region Human Supremacist
        { "Human Supremacist|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Halt heretic!!",
                    "Let me guess. Something about the emperor?",
                    "For the emperor!!"
                }
            }
        },
        { "Human Supremacist|Podarge",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Halt heretic!!",
                    "Leave me alone perv...",
                    "For the emp... wait what?"
                }
            }
        },
        #endregion
        #region Vampire Killer
        { "Vampire Killer|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Hellspring. Die now, and leave this world. You'll never belong here!",
                    "Oh but we have no intention to stay here don't you worry!",
                    "To hell with your heresy! You're nothing but a blight on mankind."
                }
            }
        },
        #endregion
        #region Witch Hunter
        { "Witch Hunter|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "We finally meet, devil worshipper.",
                    "Out of my way, old illuminated.",
                    "The outcome of our fight is already written, you cannot kill me!",
                    "We will see about that."
                }
            }
        },
        { "Witch Hunter|Floppyredoux",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "The one and only Floppyredoux.",
                    "Sigh... One of many crazy lunatics...",
                    "Begone witch! May you rot in hell for the rest of your pitiful life!",
                    "Oh, I'm so going to Expelliarmus your ass!!"
                }
            }
        },
        #endregion
        #region Exorcist
        { "Exorcist|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "So much dark energy emanating from you...",
                    "Yeah seems fair...",
                    "You corrupted fool, let me help you finding redemption!"
                }
            }
        },
        #endregion
        #region Tactical Nun
        { "Tactical Nun",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Mission failed!!"
                }
            }
        },
        { "Tactical Nun|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "[ Target acquired ]",
                    "?\nIt is too quiet, something's fishy.",
                    "[ Engaging ]"
                }
            }
        },
        { "Tactical Nun|Podarge",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "[ Target acquired ]",
                    "Am I the target?",
                    "Wait... You can hear me?",
                    "I... I can see you as well...",
                    "...",
                    "..."
                }
            }
        },
        { "Tactical Nun|Belias",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "[ Target acquired ]",
                    "Whoever might you be, you stink holiness, your cloaking tricks won't work on me.",
                    "Well then, let's gain some time!"
                }
            }
        },
        { "Tactical Nun|Cereza",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "[ Target acquired ]",
                    "Wait, I smell something familiar...",
                    "[ I... Smell?? ]",
                    "Jeanne? Could it be you?",
                    "Wait... Cereza?!!",
                    "Hi old friend! Do you like my new look?",
                    "What have you become... You disgust me!"
                },
                new List<string>()
                {
                    "[ Target acquired ]",
                    "Oh hi Jeanne! Long time no see!",
                    "Cereza? What have you become...",
                    "Oh come on don't give me that look again!! You should try to live for yourself a bit!"
                }
            }
        },
        #endregion
        #region Supreme Bishop
        { "Supreme Bishop",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Checkmate..."
                }
            }
        },
        { "Supreme Bishop|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Greetings opponent.",
                    "Yes yes, let's do it.",
                    "I have been summoned here, right before you, to stop your foolish crusade against all that is holy.",
                    "...",
                    "This very arena will be the theatre of our might and tenacity!",
                    "Yes yes...",
                    "Beware you sinful creature, this is your last warning as my will to eradicate your kind is-",
                    "Come on!! Duuuude!! Stop talking, start fighting!!"
                }
            }
        },
        { "Supreme Bishop|Podarge",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Greetings opponent.\nI have been summoned here, right before you, to stop your foolish crusade against all that is holy.",
                    "A bit chatty aren't we?",
                    "I... beg your pardon?",
                    "Oh I'm sorry, keep going.",
                    "... Well...\nThis very arena will be the theatre of our might and tenacity!",
                    "Hahaha that's ridiculous!",
                    "Is that a joke to you? This war against good and evil?",
                    "Haha yeah pretty much. Now come you hipster!"
                },
                new List<string>()
                {
                    "Greetings opponent.\nI have been summoned here... Wait... Sigh... You again?",
                    "Wow! I sure made an impression last time.",
                    "Last... time... ?",
                    "Oh I won't lose any time explaining this to you!"
                }
            }
        },
        { "Supreme Bishop|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Greetings opponent.",
                    "Are you literally a chess bishop?",
                    "You mean figuratively?",
                    "What is wrong with this game...",
                },
                new List<string>()
                {
                    "Greetings opponent.\nI have been summoned here-",
                    "Don't waste your time.",
                    "Why won't anyone let me do my fighting speech! You helling creatures are certainly not-",
                    "Ok then don't waste my time!"
                }
            }
        },
        { "Supreme Bishop|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Greetings opponent.",
                    "Oh! I like your scarf!",
                    "My scarf...?",
                    "Where did you get it?",
                    "I got it at the... Wait a minute! Why any of this would be relevant?",
                    "Haha! If I beat your ass, you give me your scarf!"
                },
                new List<string>()
                {
                    "Greetings opponent.",
                    "You didn't give me your scarf last time!",
                    "Have we met before?",
                    "Ah yes... already sick of explaining it over and over..."
                }
            }
        },
        { "Supreme Bishop|Tony",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Anthony is that you?",
                    "Oh! Hi supreme bishop! How is it going?",
                    "Anthony... What have you become...",
                    "It's just Tony now. And do I have to remind you that you are responsible of who I am today!",
                    "An alcoholic? Certainly not!!",
                    "Promoting me at the head of a brewer covenant wasn't your best idea dude!",
                    "..."
                }
            }
        },
        { "Supreme Bishop|Sir Vixid",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Greetings, sir.",
                    "Greetings, bishop.",
                    "I have been summoned here, right before you, to stop your foolish crusade against all that is holy.",
                    "And I am here to fulfill my destiny, against your holiness, in this arena which will soon become your tomb.",
                    "This very arena will be the theatre of our might and tenacity!",
                    "Indeed it will! I am more resolved than ever to eradicate your sacred being!",
                    "Beware, you sinful creature, this is your last warning as my will to eradicate your kind is unwavering!",
                    "So be it."
                }
            }
        },
        #endregion
        #region The Pop
        { "The Pop",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Bury me with my money..."
                }
            }
        },
        { "The Pop|Any",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "How dare you face the pop you devil!",
                    "The Pop? You are the creature behind all of these churches and worshipers?",
                    "I am and will always be. I'm the arm of saints, placed here to guide the humans to holiness.",
                    "Are you even human?",
                    "I am a benediction, and the last being you will see before your eternal suffering!"
                }
            }
        },
        { "The Pop|Podarge",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "How dare you face the pop you devil!",
                    "Oh SHIT an alien!!",
                    "What did you just called me?",
                    "You are not going to fool me martian!",
                    "..."
                },
                new List<string>()
                {
                    "How dare you face the pop you devil!",
                    "You were disgusting the first time, it ain't changed since...",
                    "The only disgusting being here is-",
                    "Oh my Stan! Can your arms wiggle even more when you move??",
                    "..."
                }
            }
        },
        { "The Pop|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Welcome Edam, son of Stan.",
                    "A proper welcome? What's going on here?",
                    "You are a subject of choice prince. The saints told me to take a special care of you.",
                    "How grateful of them!",
                    "Our fight will be carved in stone, the war between realms will-",
                    "Come on! You too are into lyrical stuff... Give me a break..."
                },
                new List<string>()
                {
                    "Welcome Edam, son of Stan.",
                    "Hi random Alien, son of any other random one.",
                    "Why you people keep referring to me as an alien?",
                    "The floppy arms and legs, the big head, the fucking one eye in the middle of your face? No?",
                    "Can't you recognize holy perfection?",
                    "Hahaha!! Wow this is the best one so far!"
                },
            }
        },
        { "The Pop|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "The princess of the underworld herself.",
                    "Thank you!! You are the first one to address me like so!",
                    "Don't take my words as compliments, you mere existence and presence here are nothing more than-",
                    "Annnd... Now you've ruined it."
                },
                new List<string>()
                {
                    "The princess of the underworld herself.",
                    "Yep, that's it. That's me. Let's keep it like so and start fighting!",
                    "Are you in a hurry Ivy, first of the dead?",
                    "Wow you are in fire dude! If I could be blushing I would!"
                }
            }
        },
        { "The Pop|Floppyredoux",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "What is a filthy witch doing here?",
                    "You... You took everything from me!",
                    "I don't even know who you are.",
                    "You will."
                }
            }
        },
        { "The Pop|Sir Vixid",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Oh! An actual crusader of the night, what an interesting encounter!",
                    "Finally. My destiny is finally fulfilling!",
                    "Hahaha!! Aren't your sect dead by now?",
                    "All my relatives live in me. Their wrath has feed my hate against your kind for centuries.",
                    "...",
                    "Let me give you a taste of it!"
                }
            }
        },
        #endregion
    };
}
