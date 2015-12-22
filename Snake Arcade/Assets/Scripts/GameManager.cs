using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	public static int Score = 0; 		// This is to keep score, i.e. how many foods snake ate, it is static so we can access it from another scene directly

	public GameObject SnakeHead;		// Game object to hold snake head sprite
	public GameObject SnakeBodyPart;	// This is one part (square) of snake, you will put prefab from inspector
	public GameObject Food;				// Same as above but for food

	public Text TextScore;		// This is reference to the text that displays the score in the scene

	public AudioClip FoodSound; // This is sound when snake eats food

	public float SnakeSpeed = 5; // This is how many times snake moves in one second

	List<GameObject> Snake = new List<GameObject> ();	// this is a list of all snake body parts in the scene

	Vector2 Direction = Vector2.right;	// This is a direction of snake

	bool canWeChangeDirection = true;	// This is a bool to make sure that the player cannot change direction more than once before snakes moves

	void Start () 
	{
		Score = 0;					// Initialize score to 0
		Direction = Vector2.right;	// Initialize direction to right

		TextScore = GameObject.Find ("Score").GetComponent<Text> ();	// Here we look for Object name "Score" to update it with our score

		// Next we put 3 parts of snake in the scene by Instantiating SnakePrefab
		Snake.Add (Instantiate (SnakeHead));
		Snake.Add (Instantiate (SnakeBodyPart));
		Snake.Add (Instantiate (SnakeBodyPart));

		// Next we update position for each snake part in the scene
		Snake[0].transform.position = new Vector2(-3,0);
		Snake[1].transform.position = new Vector2(-4,0);
		Snake[2].transform.position = new Vector2(-5,0);

		// Then we assign initial position for food
		Food.transform.position = new Vector2 (5, 5);

		// And then you instantiate the food prefab in scene
		Food = Instantiate (Food);

		// This is a coroutine, it is the same as classic method but you can "Pause" it for some time. 
		// In our case we pause the method for SnakeSpeed and then after that time we move snake
		StartCoroutine ("MoveSnake");
	}

	// This method "Update" is called once per frame, here we get player's input
	void Update () 
	{
		// If we cannot change direction for snake, exit the method
		if (canWeChangeDirection == false)
			return;

		// Here we get user input, when user presses some arrow key on keyboard he will change direction of snake
		if(Input.GetKey(KeyCode.UpArrow) && Direction != Vector2.down)	// If input is ArrowKey Up, and snake does not move down, we change direction to UP 
		{
			Direction = Vector2.up;			// Change direction
			canWeChangeDirection = false;	// Direction cannot be changed until snake's next move
		}
		else if(Input.GetKey(KeyCode.DownArrow) && Direction != Vector2.up)// If input is ArrowKey Down, and snake does not move up, we change direction to DOWN 
		{
			Direction = Vector2.down;			// Change direction
			canWeChangeDirection = false;	// Direction cannot be changed until snake's next move
		}
		else if(Input.GetKey(KeyCode.LeftArrow) && Direction != Vector2.right)// If input is ArrowKey Left, and snake does not move right, we change direction to LEFT 
		{
			Direction = Vector2.left;			// Change direction
			canWeChangeDirection = false;	// Direction cannot be changed until snake's next move
		}
		else if(Input.GetKey(KeyCode.RightArrow) && Direction != Vector2.left)// If input is ArrowKey Right, and snake does not move left, we change direction to RIGHT 
		{
			Direction = Vector2.right;			// Change direction
			canWeChangeDirection = false;	// Direction cannot be changed until snake's next move
		}

	}

	// This method spawns the food, the food must be at valid position i.e. it must not collide with the snake
	public void SpawnFood()
	{
		int x = 0, y = 0;	// This is used for the coordinates

		bool isValidPosition = false;	// we initialize bool to false because we want "While" loop to loop at least once

		while(isValidPosition == false)
		{
			x = Random.Range (-14, 15);	// Here we assign random coordinate for X between walls
			y = Random.Range (-14, 15); // Here we assign random coordinate for Y between walls

			isValidPosition = true;	// Now we change bool to TRUE because if food collides with the snake we will reset it to false

			// Loop through all snake parts positions and check if the food coordinates matches at one of them
			for (int i = 0; i < Snake.Count; i++) {
				if(Snake[i].transform.position.x == x && Snake[i].transform.position.y == y) // Here we ask if our X and Y is the same as the position of current snake part
				{
					isValidPosition = false;	// If food is somewhere in snake (ie it collides with snake) we reset bool to false to repeat the loop
					break;	// And here we exit FOR loop
				}
			}
		}

		Food.transform.position = new Vector2 (x, y);	//  Once we have valid position for food, we change food position with those coordinates
	}

	// This method is called on Game Over, when either snake bites itself or hit the wall
	public void GameOver()
	{
		Application.LoadLevel ("GameOver"); // This line loads GameOver scene
	}

	// When snake eats food, snake grows by 1 object
	private void GrowSnake()
	{
		Vector2 tail = Snake[Snake.Count - 1].transform.position;	// We get position of the tail 

		GameObject newTail = Instantiate (SnakeBodyPart);	// Here we instantiate new body part in the scene ang we keep the reference

		newTail.transform.position = tail;		// we put new tail at the end of the snake

		Snake.Add (newTail);	// Here we add new body part (tail) to the snake List
	}

	// This method moves the snake by making new position for each square and then updating the snake's parts positions in the scene
	private IEnumerator MoveSnake()
	{
		yield return new WaitForSeconds (1 / SnakeSpeed);	// Here we wait some time before we move the snake

		// This for loop loops through all snake parts and updates their positions, it works like this:
		// We start from the back of the snake, and for current object in the look we say that current object position is the position of the object in front of it
		for (int i = Snake.Count - 1; i > 0; i--) 
		{
			Snake[i].transform.position = Snake[i - 1].transform.position;		// In this line we assign new position for snake part
		}

		// The only thing left is that we move the head of the snake for one unit to direction 
		Snake [0].transform.position = new Vector2(Snake[0].transform.position.x + Direction.x, Snake[0].transform.position.y +Direction.y);

		// Here we check if snake bit itself
		for (int i = 1; i < Snake.Count; i++) 
		{
			// Here we check if snake's head (SnakePosition[0]) coordinate is the same as the coordinates of other snake parts
			if(Snake[0].transform.position.x == Snake[i].transform.position.x && Snake[0].transform.position.y == Snake[i].transform.position.y)
			{
				GameOver(); // If it is, it is game over
			}
		}

		// Here we check if snake hit the wall
		if (Snake[0].transform.position.x == 15 || Snake[0].transform.position.x == -15 || Snake[0].transform.position.y == 15 || Snake[0].transform.position.y == -15) 
		{
			GameOver(); // If it hits its game over
		}

		// Here we check if the snake ate food by checking if snake head's position is the same as the food position
		if (Snake[0].transform.position.x == Food.transform.position.x && Snake[0].transform.position.y == Food.transform.position.y) 
		{
			// If it is then we grow snake and spawn new food
			GrowSnake();
			SpawnFood();
			UpdateScore();
			AudioSource.PlayClipAtPoint(FoodSound, transform.position);
		}

		canWeChangeDirection = true;	// when snake moves, we finally can get player's input again

		StartCoroutine ("MoveSnake");	// Here we call again this method because we want our snake to move until game over
	}

	// This method updates score on the screen
	private void UpdateScore()
	{
		Score++;	// here we increment our score
		TextScore.text = Score.ToString ();	// and here we change text of our TextScore object with our new score, I used method ToString because Score is integer (number)
	}
}
