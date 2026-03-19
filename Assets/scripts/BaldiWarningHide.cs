using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BaldiWarningHide : MonoBehaviour
{
    public collectedisplay collecteddisplay;
    public TextMeshProUGUI messageText;
    public GameObject baldi;
    public GameObject uiiacat;
    public GameObject oggy;
    public Image baldi_frown;
    public Image baldi_talk;
    public Image baldi_rotate;
    public AudioSource typingSound;

    public float typingSpeed = 0.07f;

    private string fullText;
    private Coroutine typingCoroutine;
    
    // OPTIMIZATION 1: Cache your components so you aren't using GetComponent every frame
    private BaldiEnemy baldiScript;
    private EnemyController oggyScript;
    private UIIAController uiiacatScript;
    private UnityEngine.AI.NavMeshAgent uiiacatAgent;

    // OPTIMIZATION 2: Cache your wait times to prevent memory garbage collection spikes
    private WaitForSeconds standardDelay;
    private WaitForSeconds punctuationDelay;

    // OPTIMIZATION 3: Use a struct and arrays to store your quotes cleanly
    private struct Quote
    {
        public string text;
        public int fontSize;
        public Quote(string t, int f) { text = t; fontSize = f; }
    }

    private readonly Quote[] homeworkQuotes = new Quote[]
    {
        new Quote("Are you retarded because the answer is wrong…\nor is the answer wrong because you are retarded?", 30),
        new Quote("I am the one who grades", 50),
        new Quote("I am not crazy! \nI know you swapped those numbers!", 40),
        new Quote("I don’t like repeating myself… \nespecially mistakes.", 40),
        new Quote("I was having a good day. \nYou changed that.", 40),
        new Quote("I see we’re skipping the \n“learning” part today.", 40),
        new Quote("Class dismissed… \nfor you.", 50),
        new Quote("Think, Player I! \nThink!", 50),
        new Quote("My students calculate!\nMy students concentrate!\n My students solve!!", 40),
        new Quote("I would have written math problems for you…\nbut you are not nerdy enough to solve them", 33),
        new Quote("I have graded these papers before!", 40),
        new Quote("I’ll take a notebook… \nand grade it.", 40),
        new Quote("What kind of problems are your type?", 40),
        new Quote("Player I, what is this behaviour?", 40),
        new Quote("call cru cya bacha?", 30),
        new Quote("TATAKAI!!!", 55),
        new Quote("Throughout halls and classrooms… \nI alone grade the answers. ", 40),
        new Quote("This classroom shall know decimals. ", 40),
        new Quote("Hakuna Mistaka.", 60),
        new Quote("Notebook, notebook on the desk… \nwho’s the smartest of them all?.....\nnot you.", 40),
        new Quote("I can do this all day. ", 50),
        new Quote("To solve… or not to solve… \nthat is the question. ", 40),
        new Quote("With great guesses \ncomes great consequences ", 50),
        new Quote("The Force may guide you… \nbut the answer key decides. ", 40),
        new Quote("You were the chosen one. \nYou were meant to solve them. ", 40),
        new Quote("Smells Like Wrong Answers.", 50),
        new Quote("We will, We will, \nSlap you", 40),
        new Quote("New Notebook,\nSame Old Mistakes ", 50),
        new Quote("Is it 2? Is it 2? Tell me now.. \nIt's a promise I can't make \nand I won't validate", 40),
        new Quote("Do I Wanna Solve? ", 50),
        new Quote("What the hell am I grading here? \nI don't belong here...", 40),
        new Quote("It’s Me. Hi. \nI’m the Problem, It’s 3. \nAt tea time, \nEverybody agrees.", 35),
        new Quote("You either solve quick enough, \nor live long enough to hear the ruler. ", 40),
        new Quote("Your answer is kinda hopeless", 50),
        new Quote("skill issue.", 50),
        new Quote("Bro forgot to carry the one.", 50),
        new Quote("Calculator detected. \nOpinion rejected.", 50),
        new Quote("\"2 + 2 = 5?\" Are you trying to put the world to right?", 50),
        new Quote("TBH mid answer", 50),
        new Quote("Stop spamming 6 and 7", 50),
        new Quote("It's grading time!", 50),
        new Quote("Listen here you stupid little punk, \njust solve these questions", 40),
        new Quote("Get in the classroom, Shinji.", 50),
        new Quote("Bankai: Konjiki Ashisogi Jizo.", 55),
        new Quote("Congratulations! \n(You failed)", 50),
        new Quote("Domain Expansion: Malevolent Quiz.", 55),
        new Quote("Bro's math ain't mathing.", 50),
        new Quote("Let him cook? \nNah, bro burnt the notebook.", 45),
        new Quote("Your GPA is dropping faster \nthan your frame rate.", 40),
        new Quote("Pythagoras is rolling in \nhis grave right now.", 40),
        new Quote("Swiper, no swiping!!", 50),
        new Quote("I am rapidly \napproaching your location.", 50),
        new Quote("Your chances of survival are approaching zero... as a limit.", 40),
        new Quote("You can't even solve a double integral, \nyet you challenge me?!", 40),
        new Quote("Did you just forget \nthe constant of integration?!", 50),
        new Quote("Even a shadow clone could solve this!", 45),
        new Quote("Chat, is this answer real?", 50),
        new Quote("Bro thought he could \nspeedrun the syllabus.", 45),
        new Quote("Art is an explosion! \nBut your math is just a disaster.", 45),
        new Quote("Now we got bad blood... \nand bad grades.", 50),
        new Quote("Hello darkness my old friend, \nI've come to grade your test again.", 35),
        new Quote("Bazinga! That answer was completely wrong.", 50),
        new Quote("I'm not crazy, \nmy mother had my math skills tested!", 40),
        new Quote("That equation is wrong \non a molecular level.", 45),
        new Quote("10 points from your grade \nfor that atrocious math.", 45),
        new Quote("It's levi-O-sa, not levi-o-SA... \nand the answer is 6, not 7!", 35),
        new Quote("You can't just pay to \nwin with Robux.", 45),
        new Quote("Touching Grass Simulator: \nDetention Edition.", 45),
        new Quote("Did you just lick the paper \ninstead of solving for X?", 40),
        new Quote("I'll give you a slap for every wrong answer.", 45),
        new Quote("The VIPs are not impressed with this math.", 45),
        new Quote("Difficulty: Seven of Diamonds. \nAnd you still failed.", 45),
        new Quote("Google En Passant... \nthen Google how to solve this.", 40),
        new Quote("Holy hell! \nWhat a massive blunder.", 50),
        new Quote("You sacrificed THE ROOOK!!? \nFOR A WRONG ANSWER??!", 45),
        new Quote("Pawn structure is weak, \nmath structure is weaker.", 40),
        new Quote("I gently open the notebook....", 50),
        new Quote("Every day, I imagine a future where \nyou can actually do math.", 35),
        new Quote("Will you calculate the way into my heart? \nNo. You won't.", 40),
        new Quote("WAS THAT THE GRADE OF '87?!", 55),
        new Quote("I always come back... \nto grade your papers.", 45),
        new Quote("Please deposit five coins... \nto recalculate your GPA.", 40),
        new Quote("I guess love can't save \nyou from a quiz.", 45),
        new Quote("I'm putting a giant X over your \nface until you solve this.", 40),
        new Quote("Please let me keep this one memory... \nof the quadratic formula.", 35),
        new Quote("It's a story of a boy meets a test. \nBut you should know upfront, \nthis is not a passing grade.", 35),
        new Quote("Good morning, and in case I don't see ya, \ngood afternoon, good evening, and \ngood luck on the re-test!", 30),
        new Quote("We accept the grades \nwe think we deserve.", 40),
        new Quote("Was anything real? You were real. \nBut your math was totally fake.", 40),
        new Quote("And in that moment, I swear... \nyour answer was infinite. \nAnd wrong.", 35),
        new Quote("I love The Smiths. \nBut I absolutely hate your math.", 45),
        new Quote("I just woke up one day and I knew... \nthat you were going to fail.", 40),
        new Quote("You didn't think anyone actually noticed your bad grades. \nBut I did.", 40),
        new Quote("I know these will all be stories someday, \nbut right now, you're failing.", 35),
        new Quote("The opportunity to pass was dangling right in front of your eyes!", 45),
        new Quote("I am the god of matchmaking... \nyour terrible math with a failing grade.", 35),
        new Quote("I've been watching you from \nthe proxy-proxy-proxy dimension, \nand you're still wrong.", 35),
        new Quote("I know who the real killer is... \nit's your terrible long division.", 40),
        new Quote("I am ten billion percent sure you failed this test.", 45),
        new Quote("This math is so primitive, \nit belongs in the Stone Age!", 45),
        new Quote("We're going to use the power of science \nto fix your atrocious grades.", 40),
        new Quote("Your intelligence was petrified \n3,700 years ago, wasn't it?", 40),
        new Quote("Are you a rock, a wolf, \nor just a student who didn't study?", 40),
        new Quote("The Nokkers are destroying your \nmemories of basic algebra!", 40),
        new Quote("You have no enemies... \nexcept for this math test.", 45),
        new Quote("This answer is as incomplete \nas my physical body.", 40),
        new Quote("I'm popping a Sandevistan \njust to grade this faster.", 40),
        new Quote("Mama mia! What is this \natrocity of an equation?!", 45),
        new Quote("Nobody, nobody, nobody... \nunderstands this equation.", 40),
        new Quote("I bet on losing dogs, \nI'm betting on you twin.", 45),
        new Quote("I'm so sick of... \ngrading these wrong answers.", 45),
        new Quote("I haven't looked at the sun for so long... \nI've been grading this garbage.", 40),
        new Quote("Yesterday, all my troubles seemed so far away... \nthen I saw your test paper.", 35),
        new Quote("You're a creep. You're a weirdo. \nWhat the hell is this answer doing here?", 35),
        new Quote("Karma police, arrest this student. \nTheir math is making me feel ill.", 35),
        new Quote("Everything in its right place... \nexcept your completely wrong decimal point.", 45),
        new Quote("You have not been paying attention! \nPaying attention, paying attention!", 40),
        new Quote("Fake plastic trees, \nfake plastic numbers, \ncompletely fake plastic answer.", 40),
        new Quote("Give me reasons we should be complete... \nbecause this equation isn't.", 40),
        new Quote("Is this it? \nIs this the best math you can do?", 50),
        new Quote("Chief, our village is under attack... \nby your atrocious math!", 40),
        new Quote("You rushed your Town Hall, \nand you definitely rushed this test.", 40),
        new Quote("I'm sending this test paper \nstraight to the Derby trash.", 45),
        new Quote("On that day, mankind received a grim reminder... \nof your terrible math scores.", 30),
        new Quote("Thank you for wrapping this scarf around me... \nbut you still get an F.", 35),
        new Quote("I'm the Armored Teacher, \nand he's the Colossal Grader.", 40),
        new Quote("Not even a hoverboard can \nsave you from this crash.", 45),
        new Quote("Super Sneakers won't help you \njump over this F.", 45),
        new Quote("New High Score! \nFor the most incorrectly \nanswered questions.", 40),
        new Quote("The probability of you passing \nthis class is \nstatistically insignificant.", 35),
        new Quote("I'm about to enter the \nanaphase of my absolute rage!", 45),
        new Quote("Even a single-celled organism \ncould pass this class!", 45),
        new Quote("The Krebs Cycle generates more energy \nthan you put into this test.", 35),
        new Quote("I am forcefully shutting down \nyour electron transport chain!", 40),
        new Quote("You still believe the Earth is the center of the universe? \nJust like you think this answer is right!", 30),
        new Quote("This math is pure heresy! \nI'm sending you to the Inquisition!", 40),
        new Quote("You will be burned at the stake \nfor this atrocious calculation!", 40),
        new Quote("Look at the stars, Player I... \nand realize how small your GPA is.", 35),
        new Quote("Even under torture, I wouldn't confess \nthat this answer is correct.", 40),
        new Quote("Chunin Exam rules: \nAnyone caught writing this \ngarbage is disqualified!", 40),
        new Quote("I'm just a walrus driving a taxi, \nand even I know this math is wrong.", 40),
        new Quote("You want to go viral? \nTry getting a passing grade first.", 40),
        new Quote("Are we playing a gacha game? \nBecause you just pulled a zero.", 40),
        new Quote("Did the Serpo aliens \nabduct your brain cells?!", 45),
        new Quote("An alien invasion makes more sense \nthan whatever you wrote here.", 40),
        new Quote("Okarun! Help me beat some \nmath into this kid!", 45),
        new Quote("Hikaru would have known \nthe answer to this.", 45),
        new Quote("Present day, \nPresent time.", 40),
        new Quote("Let's all love Lain... ", 40),
        new Quote("This answer doesn't exist \nin the physical world \nor the Wired.", 40),
        new Quote("God is in the Wired, \nand he's judging your math.", 45),
        new Quote("YOWAI MO", 50),
        new Quote("I am the Math Devil, \nand I've come to claim your GPA!", 40),
        new Quote("Makima is watching... \nand she is deeply disappointed.", 40),
        new Quote("If you were a Hashira, \nyou'd be the Pillar of Stupidity.", 45),
        new Quote("Cosplaying as a good student \nwon't make you one.", 45),
        new Quote("You wuv this? \nWell, I wuv giving you an F.", 45)
    };

    private readonly Quote[] chalkQuotes = new Quote[]
    {
        new Quote("You can't keep getting away with it!", 45),
        new Quote("I will grade your wife. \nI will grade your son. \nI will grade your infant daughter", 40),
        new Quote("BAKA!!!", 80),
        new Quote("No chalk left to teach… \nguess I’ll demonstrate instead.", 50),
        new Quote("Stay out of my territory.", 45),
        new Quote("Running won’t improve your grade.", 45),
        new Quote("I can always start again… \nteach another student.", 45),
        new Quote("Japan is turning footsteps into electricity! Using piezoelectric tiles, every step you take generates a small amount of energy. Millions of steps together can power LED lights and displays in busy places like Shibuya Station. A brilliant way to create a sustainable and smart city -- turning movement into clean, renewable energy.", 28),
        new Quote("mime level joke...", 50),
        new Quote("NAH, I’D WIN.", 60),
        new Quote("this is my perfect victory!", 50),
        new Quote("You are my special... \nreal special", 45),
        new Quote("Know your place, fool!", 50),
        new Quote("No running in the halls!", 50),
        new Quote("Are you sure?", 60),
        new Quote("Say my name.", 60),
        new Quote("Yeah, Maths!", 60),
        new Quote("Fluorescent Assessment. ", 50),
        new Quote("Look what you made me grade.", 50),
        new Quote("We are never, ever, ever \ngetting this equation right.", 40),
        new Quote("Detention speedrun any%. ", 55),
        new Quote("Bro fighting for his life \nagainst basic addition.", 40),
        new Quote("Partial credit? \nIn this economy? ", 50),
        new Quote("Blud really thought.", 50),
        new Quote("You moving through these halls \nlike NPC behavior. ", 40),
        new Quote("Take it to DMs \n(Detention Monitoring Space)", 40),
        new Quote("Across all universes… \nyou still didn’t carry the one.", 40),
        new Quote("lite lo", 50),
        new Quote("WRONG!    WRONG!\nWRONG!    WRONG! ", 50),
        new Quote("You’re approaching me? ", 50),
        new Quote("It was me, \nBALDI!", 60),
        new Quote("NANI?! ", 80),
        new Quote("To be continued...", 50),
        new Quote("You know the guessing… \n and I know the answer. \nI was thinking… maybe you and I… \n could stop pretending.", 30),
        new Quote("L + ratio", 60),
        new Quote("Yeah, Maths!", 60),
        new Quote("Erm, what the sigma?", 60),
        new Quote("Did you just divide by \nzero in my halls?!", 50),
        new Quote("Is this mitosis? \nBecause you're about to split.", 45),
        new Quote("Null pointer exception: Intelligence not found.", 45),
        new Quote("You're running away again, \naren't you?", 50),
        new Quote("Do not fear the test. \nFear me.", 55),
        new Quote("I will show you the true power of the Uchiha... \nI mean, Mathematics.", 40),
        new Quote("You just dug straight down \ninto an F.", 45),
        new Quote("Not even a Totem of Undying \ncan save your GPA.", 40),
        new Quote("Soft kitty, warm kitty, \nlittle ball of failure.", 45),
        new Quote("You're a failure, \nPlayer I.", 55),
        new Quote("The wand chooses the wizard, \nand the ruler chooses you.", 40),
        new Quote("OOF.", 100),
        new Quote("I'm about to reset your character.", 50),
        new Quote("Bro is lagging in real life.", 50),
        new Quote("Player I... eliminated.", 50),
        new Quote("Your visa has expired, \nand so has your time limit.", 40),
        new Quote("Your Elo must be \nin the double digits.", 50),
        new Quote("Just Baldi.", 60),
        new Quote("You're gonna have to try \na little harder than THAT.", 45),
        new Quote("Cigarettes out the window, \nand your GPA down the drain.", 45),
        new Quote("Meet me in Montauk... \nafter detention.", 50),
        new Quote("The first rule of Math Club is: \nYou do not talk about Math Club.", 40),
        new Quote("You are not your GPA.", 55),
        new Quote("I'm not a concept, I'm just an angry teacher looking for a correct answer.", 35),
        new Quote("You met me at a very strange time in the semester.", 40),
        new Quote("You're looking for the rose-colored campus life? \nTry finding the right answer first.", 40),
        new Quote("If you want a different outcome, \nmaybe study instead of joining another club.", 35),
        new Quote("The thread above your head is about to snap, \njust like my patience.", 40),
        new Quote("I will take the form of the thing \nthat hurts you the most... a quiz.", 40),
        new Quote("Even a nameless orb learns faster than you do.", 45),
        new Quote("A true warrior doesn't need a calculator.", 45),
        new Quote("You want revenge? \nTry getting a passing grade first.", 40),
        new Quote("My math skills were stolen by demons... \nwhat's your excuse?", 40),
        new Quote("Built different? Nah, \nyour math is just built wrong.", 45),
        new Quote("You're going cyberpsycho over basic calculus!", 50),
        new Quote("Fly me to the moon? \nFly yourself to the library!", 40),
        new Quote("All you had to do was follow the damn formula, CJ!", 45),
        new Quote("Ah shit, here we go again... ", 45),
        new Quote("You picked the wrong answer, fool!", 50),
        new Quote("See you around, officer... \nI mean, failing student.", 45),
        new Quote("You need to hit the gym... \nand by gym I mean the textbook.", 40),
        new Quote("Respect -100. Grade: F.", 55),
        new Quote("You keep waiting for the long piece, \nbut the right answer isn't coming.", 35),
        new Quote("Boom! Tetris for me, \nan F for you.", 50),
        new Quote("You dropped the chalk harder than a misplaced T-spin.", 40),
        new Quote("Thank you Player I! \nBut your correct answer is in another castle!", 35),
        new Quote("Not even a 1-Up Mushroom can revive this grade.", 45),
        new Quote("I knew you were trouble when you guessed that answer.", 40),
        new Quote("I'm the bad guy... duh.", 50),
        new Quote("So you're a tough guy, really bad at math guy.", 40),
        new Quote("Birds of a feather, \nfail together I know.", 50),
        new Quote("Not a lot, just forever... \nin detention.", 45),
        new Quote("Do you have something against dogs? \nOr just against passing grades?", 40),
        new Quote("It doesn't have to be like this... \nbut your math is terrible.", 45),
        new Quote("Drunk drivers, killer whales, and \nstudents who can't do fractions.", 35),
        new Quote("You have no right to be depressed... \nyou haven't even seen your test score yet.", 35),
        new Quote("You got some nice shoulders... \ntoo bad the brain above them can't multiply.", 35),
        new Quote("Hey dude, don't make it bad... \nactually, this grade is already terrible.", 40),
        new Quote("Here comes your F, \nDoo Doo - Doo", 45),
        new Quote("Let it be?\n No, I will NOT let this be!", 50),
        new Quote("You do it to yourself, you do... \nand that's what really hurts.", 35),
        new Quote("I don't wanna slow dance in the dark, I want you to solve for X.", 40),
        new Quote("Yeah right, yeah right... \nlike you actually studied for this.", 45),
        new Quote("Cause sometimes I look in your eyes... \nand see no glipmse of math skills.", 35),
        new Quote("The room is on fire as you're calculating your own doom.", 40),
        new Quote("Last nite, she said... \nyou don't know anything about algebra.", 40),
        new Quote("I missed the last bus, \nand you missed the entire point of the lesson.", 40),
        new Quote("I'll try, but you see, \nit's hard to explain", 45),
        new Quote("The adults are talking... \nabout how terrible your grades are.", 40),
        new Quote("You only live once, \nso why spend it failing math?", 45),
        new Quote("I love Arpit Bala thongs.", 45),
        new Quote("Not even a max-level Healing Spell \ncan fix this grade.", 45),
        new Quote("You dropped a 0-star attack \non this math problem.", 45),
        new Quote("My Wall Breakers have more \nintelligence than this answer.", 40),
        new Quote("I'm kicking you out of the \nClan for this calculation.", 45),
        new Quote("Your base is getting 100% destroyed, \njust like your academic career.", 35),
        new Quote("Maths worse than \nNamit's jokes...", 40),
        new Quote("Double it and give it \nto the next person", 40),
        new Quote("HEHEHEHA! ", 60),
        new Quote("That answer was a massive \nnegative Elixir trade.", 45),
        new Quote("I'm about to drop a \nMega Knight right on you.", 45),
        new Quote("You just got 3-crowned \nby basic arithmetic.", 50),
        new Quote("Grrrrr... ", 60),
        new Quote("Stop bush camping and \nactually solve the equation!", 45),
        new Quote("You jumped into the enemy team \n with 10 gems and 0 math skills.", 40),
        new Quote("Even a toxic Edgar main \ncalculates better than this.", 40),
        new Quote("I'm blasting you with a \nShelly Super for that answer.", 45),
        new Quote("You got bad randoms? \nNo, you ARE the bad random.", 45),
        new Quote("Legendary Starr Drop? \nNo, just a Rare F.", 50),
        new Quote("Even Greg wouldn't buy this \nterrible answer from your roadside shop!", 40),
        new Quote("Tom the delivery boy can't fetch you a better GPA.", 45),
        new Quote("Did you study from \nWhatsapp University?", 45),
        new Quote("The propaganda of you passing \nthis class ends today.", 45),
        new Quote("I have made a detailed \nanalysis of your failure.", 45),
        new Quote("Logical fallacy detected \nin your geometry!", 50),
        new Quote("Talk no Jutsu won't save you \nfrom a failing grade.", 45),
        new Quote("Wake up to reality... \nyou failed the exam.", 50),
        new Quote("This is my ninja way: \ngiving you a zero.", 45)
    };

    void Start()
    {
        // Grab the references once at the start of the game
        if (baldi != null) baldiScript = baldi.GetComponent<BaldiEnemy>();
        if (oggy != null) oggyScript = oggy.GetComponent<EnemyController>();
        if (uiiacat != null)
        {
            uiiacatScript = uiiacat.GetComponent<UIIAController>();
            uiiacatAgent = uiiacat.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        // Initialize the wait times once
        standardDelay = new WaitForSeconds(typingSpeed);
        punctuationDelay = new WaitForSeconds(0.25f);
    }

    public void ShowWarning()
    {
        gameObject.SetActive(true);
        CancelInvoke("HideBaldiWarning"); // Cancels any lingering hide timers
        baldi_frown.enabled = false;
        baldi_rotate.enabled = false;
        baldi_talk.enabled = false;
        Invoke("HideBaldiWarning", 5f);
    }

    public void HideBaldiWarning()
    {
        baldi_frown.enabled = false;
        baldi_talk.enabled = false;
        baldi_rotate.enabled = false;
        gameObject.SetActive(false);   // hide the canvas
        
        RectTransform rt = messageText.rectTransform;
        rt.anchoredPosition = new Vector2(170f, rt.anchoredPosition.y);
    }

    public void WarningNumber(GameObject item)
    {
        if (item.CompareTag("Homework"))
        {
            if (collecteddisplay.homework >= 4 && !baldiScript.isEnraged)
            {
                messageText.text = "*strikes*";
                messageText.fontSize = 120;
                baldi_frown.enabled = true;

                baldiScript.lookRadius = 1000f;
                baldiScript.isEnraged = true;
            }
            else
            {
                // Grab a random quote from the array
                Quote q = homeworkQuotes[Random.Range(0, homeworkQuotes.Length)];
                messageText.text = q.text;
                messageText.fontSize = q.fontSize;
                baldi_talk.enabled = true;
            }
        }
        else if (item.CompareTag("Chalk"))
        {
            if (collecteddisplay.chalk >= 4 && !oggyScript.isEnraged)
            {
                messageText.text = "*meow*";
                messageText.fontSize = 120;
                RectTransform rt = messageText.rectTransform;
                rt.anchoredPosition = new Vector2(0f, rt.anchoredPosition.y);

                oggyScript.lookRadius = 500f;
                oggyScript.isEnraged = true;

                if (!uiiacatScript.isEnraged)
                {
                    uiiacatScript.isEnraged = true;
                    uiiacatAgent.speed *= 1.5f;
                    uiiacatScript.wallLifetime *= 1.5f;
                }
            }
            else
            {
                // Grab a random quote from the array
                Quote q = chalkQuotes[Random.Range(0, chalkQuotes.Length)];
                messageText.text = q.text;
                messageText.fontSize = q.fontSize;
                baldi_rotate.enabled = true;
            }
        }

        fullText = messageText.text;
        messageText.maxVisibleCharacters = 0;

        // Stop the previous typing effect if it's still running
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        int totalVisibleChars = fullText.Length;

        for (int visibleCount = 1; visibleCount <= totalVisibleChars; visibleCount++)
        {
            messageText.maxVisibleCharacters = visibleCount;

            char currentChar = fullText[visibleCount - 1];

            if (!char.IsWhiteSpace(currentChar))
                typingSound.Play();

            // Use the cached wait times here instead of creating new ones
            yield return currentChar == '.' || currentChar == ',' || currentChar == '!'
                ? punctuationDelay
                : standardDelay;
        }
    }
}