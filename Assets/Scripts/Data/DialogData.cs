using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogData
{
    // ' ' separates words (no pause).
    // '. ' Pause
    // <P> Pause
    public static Dictionary<string, List<List<string>>> DialogTree = new Dictionary<string, List<List<string>>>()
    {
        #region Harpy
        { "Harpy|Edam",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Master???",
                    "You traitor!!"
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
                    "The fuck you're talking about?",
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
                    "Go back where you come from vermin! Those gates have remained sealed for centuries and I am here to keep it that way."
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
                    "Snort..."
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
                    "It just feels like... I'm already bored of it even it hasn't begun... Have you done something against the time continuum???",
                    "Hehehe!!!"
                },
                new List<string>()
                {
                    "Hmm... Why am I not surprised to see you here milady?",
                    "Might be linked to the fact that we have already done it several times maybe!!",
                    "Wait... No!!! The amulet ?",
                    "Haha, seeing you this surprised each time is pleasing!!"
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
                    "Edam... Seeing you here seems like a deja vu ! I guess I've already failed before didn't I ?",
                    "Haha! You've been a pleasing opponent so far don't worry.",
                    "I see... I hate this smile of yours. Let me put an end to it!!"
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
        { "Stan|Ivy",
            new List<List<string>>()
            {
                new List<string>()
                {
                    "Daughter. Seeing you here fills my heart with sadness...",
                    "Haha don't even try it. I know I have no place in your heart!",
                    "...\nWhat are you doing here Ivy?",
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
                    "Don't tell me this like it's nothing...",
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
                    "I came here to... Wait! You remember our fight?",
                    "Do you think a simple trick like this amulet would let me unaware of what's happening?",
                    "You call that a simple trick? It seems to be well enough for all the simple minded we fight each time!",
                    "Don't underestimate your overlord!!"
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
    };
}
