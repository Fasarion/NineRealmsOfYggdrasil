
    [System.Serializable]
    public class Dialogue
    {
        //But essentially, I think we want to pass in the text from the dialogue system into the Dialogue class, or perhaps the sentences class.
        //We're gonna want to pass the nodes form the dialogue system into a list
        //We're going to have to package the information about a nodes connections to the 
        
        //Is it possible we can just look at the graph and see what our currentNode in the graph is and when that will determine which dialogue is being played.
        //Then we enqueue the dialogue that is next in line to be played 
        
        
        //A dialogue is opened from our dialogue manager, i.e. some NPC is spoken to.

        //Option 1:
        //1. Define a variable for keeping track of the currentNode in the graph.
        //2. Call the Dialogue system and tell it to start playing through dialogues nodes from its startingNode.
        //3. Set the currentNode to be startingNode.
        //4. Send the node information from that node back to the dialogue manager, including its branches if it has any.
        //5. The dialogue itself is assigned to the sentences variable as a single sentence.
        //6. The dialogue is played by the DialogueManager
        //7. When the dialogue is completed, instead of closing the dialogue right away, we send a 
        //message to the DialogueGraph to move the currentNode pointer to the next node and leave the dialogue active.
        //8. If the pointer can move to the next node, we add the next dialogue to the dialogueManager and keep playing the dialogue.
        //9. If the pointer reaches a branching dialouge, the branch options menu will open to the player.
        //10. When the player chooses a branch, the pointer is updated to the first node in that branch.
        //11. We proceed as before until we reach the end of the node tree.
        //12. Upon reaching the end of the node tree, we close the dialogue window.
         
        //Option 2:
        //1. Define a variable for keeping track of the currentNode in the graph.
        //2. Check all nodes following the currentNode until you reach a branch and add them to a playlist.
        //3. Call the Dialogue system and tell it to start playing through the nodes added to the list starting from its startingNode.
        //4. Set the currentNode to be startingNode.
        //5. Add the list to a dialogue object as a set of sentences 
        //6. Send that information back to the dialogue manager.
        //7. The list of sentences from the dialogue are added to the playback system as it is.
        //8. The dialogue is played by the DialogueManager until it reaches a branch or stops.
        //9. When a sentence is completed, instead of closing the dialogue right away, we send a 
        //message to the DialogueGraph to move the currentNode pointer to the next node.
        //10. If the pointer can move to the next node, we keep playing the sentences.
        //11. If the pointer reaches a branching dialouge, the branch options menu will open to the player.
        //12. When the player chooses a branch, the pointer is updated to the first node in that branch.
        //13. We run the logic for adding a stretch of nodes to the playback system again.
        //14. We proceed as before until we reach a new branch (or the end of the node tree).
        //15. Upon reaching the end of the node tree, we close the dialogue window.
        
        //Side note. We want to be able to shift between different node groups based on parameters being set by the user for how and when the dialogues are supposed to be fired.
        //For simplicity's sake it might be prudent to allow the user to specify what condition will allow the dialogue to shift which group to play.
        
        //For looping dialogue, the player must be able to close the dialogue window on their own. 
        //Easily fixed with a branch that doesn't loop, but might be good to have a failsafe.
        
        //We might be able to appropriate the Dialogue Containers for our purpose.
        //For starters we could make a version that uses the dialogue system graph as its information access point.
        //The graph would need callbacks from the Dialogue Manager however to transition its pointer to the next node.
        //Which means it would need a reference to the current graph.
        //Which means we would need some kind of object that can load the contents of the graph into memory at runtime.
        //I wonder how I should do to load the graph into memory.

        public Dialogue(Sentence[] sentences)
        {
            this.sentences = sentences;
        }


        public Sentence[] sentences;
    }
