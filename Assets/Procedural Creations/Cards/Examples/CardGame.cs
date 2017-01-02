using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardGame : MonoBehaviour
{
	public CardDeck Deck;
	//List<CardDefinition> m_deck = new List<CardDefinition>();

	List<Card> m_dealer = new List<Card>();

	List<Card> m_top = new List<Card>();
	List<Card> m_top_cards_to_pass = new List<Card>();
	List<CardLogistInfo> m_top_cards_to_pass_positions = new List<CardLogistInfo>();
	int topScore;

	List<Card> m_left = new List<Card>();
	List<Card> m_left_cards_to_pass = new List<Card>();
	List<CardLogistInfo> m_left_cards_to_pass_positions = new List<CardLogistInfo>();
	int leftScore;

	List<Card> m_right = new List<Card>();
	List<Card> m_right_cards_to_pass = new List<Card>();
	List<CardLogistInfo> m_right_cards_to_pass_positions = new List<CardLogistInfo>();
	int rightScore;

	List<Card> m_player = new List<Card>();
	List<Card> m_player_cards_to_pass = new List<Card>();
	List<CardLogistInfo> m_player_cards_to_pass_positions = new List<CardLogistInfo>();
	bool playerPlayedCorrectly = false;
	int playerScore;
	
	Card[] cardsOnTable = new Card [4];

	GameObject PlayerWins;
	GameObject DealerWins;
	GameObject NobodyWins;
	
	enum GameState
	{
		Invalid,
		
		Started,
		PlayerBusted,
		Resolving,
		
		DealerWins,
		PlayerWins,
		RightWins,
		TopWins,
		LeftWins,

		SelectingCards,
		PassingCards,
	};
	
	enum Turn
	{
		player,
		right,
		top,
		left,
	};

	GameState m_state;
	
	
	Turn starting_side;
	Turn shuffler;
	
	GameObject[] Buttons;
	
	
	
	// Use this for initialization
	void Start ()
	{
		shuffler = Turn.player;
		starting_side = Turn.right;
		m_state = GameState.Invalid;
		
		playerScore = 0;
		leftScore = 0;
		rightScore = 0;
		topScore = 0;
		
		Deck.Initialize();
		PlayerWins = this.transform.Find("MessagePlayerWins").gameObject;
		DealerWins = this.transform.Find("MessageDealerWins").gameObject;
		NobodyWins = this.transform.Find("MessageTie").gameObject;
		PlayerWins.SetActive(false);
		DealerWins.SetActive(false);
		NobodyWins.SetActive(false);
		
		Buttons = new GameObject[3];
		Buttons[0] = this.transform.Find("Button1").gameObject;
		Buttons[1] = this.transform.Find("Button2").gameObject;
		Buttons[2] = this.transform.Find("Button3").gameObject;
		//UpdateButtons();
	}
	
	void UpdateButtons()
	{
		Buttons[0].GetComponent<Renderer>().material.color = Color.blue;
		Buttons[1].GetComponent<Renderer>().material.color = (m_state == GameState.Started) ? Color.blue : Color.red;
		Buttons[2].GetComponent<Renderer>().material.color = (m_state == GameState.Started || m_state == GameState.PlayerBusted) ? Color.blue : Color.red;
	}
	
	void ShowMessage(string msg)
	{
		if (msg == "Dealer")
		{
			PlayerWins.SetActive(false);
			DealerWins.SetActive(true);
			NobodyWins.SetActive(false);
		}
		else if (msg == "Player")
		{
			PlayerWins.SetActive(true);
			DealerWins.SetActive(false);
			NobodyWins.SetActive(false);
		}
		else if (msg == "Nobody")
		{
			PlayerWins.SetActive(false);
			DealerWins.SetActive(false);
			NobodyWins.SetActive(true);
		}
		else
		{
			PlayerWins.SetActive(false);
			DealerWins.SetActive(false);
			NobodyWins.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (Input.GetMouseButtonDown (0))
		{
			
			RaycastHit hit = new RaycastHit ();
			bool isHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit);
			
			if (m_state == GameState.SelectingCards)
			{		
				if (isHit)
				{
					if (hit.collider.gameObject.name == "Card")
					{
						StartCoroutine (onPlayerSelectingCards (hit));
					}
				}
			}
			
			else if (m_state == GameState.Started)
			{
				if (isHit)
				{
					if (hit.collider.gameObject.name == "Card")
					{
						playerPlayedCorrectly = true;
						onPlayerPlaying(hit);
					}
				}
				
			}
		}

		//UpdateButtons();
	}
		


	void Shuffle()
	{
		if (m_state != GameState.Invalid)
		{
		}
	}


	
	
	Vector3 GetDeckPosition()
	{
		return new UnityEngine.Vector3(-17,0,0);
	}
	
	const float FlyTime = 0.2f;
	
	

	void dealRight()
	{
		CardDef cRight = Deck.Pop ();
		
		if (cRight != null)
		{
			Debug.Log ("Deck-Popped for right");
			GameObject newObj = new GameObject ();
			newObj.name = "Card";
			Card newCard = newObj.AddComponent (typeof(Card)) as Card;
			newCard.Definition = cRight;
			newObj.transform.parent = Deck.transform;
			newCard.TryBuild ();

			float x;
			float y;
			float z;

			Quaternion rot = transform.rotation;	
			rot = Quaternion.Euler (0, 0, 90);

			if (m_right.Count < 4) 
			{
				x =5;
				y = -2 + (m_right.Count) * 1.5f;
				z = (m_right.Count) * -0.1f;
				newCard.transform.rotation = rot;
			} 
			else if (m_right.Count < 8) 
			{
				x = 6;
				y = -2 + (m_right.Count%4) * 1.5f;
				z = (m_right.Count) * -0.1f;	
				newCard.transform.rotation = rot;
			} 
			else 
			{
				x = 7;
				y = -2.75f + (m_right.Count%8) * 1.5f;
				z = (m_right.Count) * -0.1f;
				newCard.transform.rotation = rot;
			}

			m_right.Add (newCard);
			Vector3 deckPos = GetDeckPosition ();
			newCard.transform.position = deckPos;
			newCard.SetFlyTarget (deckPos, new Vector3 (x, y, z), FlyTime);
		}
	}
	
		
	
	void dealTop ()
	{
		CardDef cTop = Deck.Pop ();
		if (cTop != null) 
		{
			Debug.Log ("Deck-Popped for top");
			GameObject newObj = new GameObject ();
			newObj.name = "Card";
			Card newCard = newObj.AddComponent (typeof(Card)) as Card;
			newCard.Definition = cTop;
			newObj.transform.parent = Deck.transform;
			newCard.TryBuild ();
			
			float x;
			float y;
			float z;

			if (m_top.Count < 4) 
			{
				x = -3 + (m_top.Count) * 1.5f;
				y = 3;
				z = (m_top.Count) * -0.1f;
			} 
			else if (m_top.Count < 8) 
			{
				x = -3 + (m_top.Count % 4) * 1.5f;
				y = 4;
				z = (m_top.Count) * -0.1f;	
			} 
			else {
				x = -3.75f + (m_top.Count % 8) * 1.5f;
				y = 5;
				z = (m_top.Count) * -0.1f;
			}
			
			m_top.Add (newCard);
			Vector3 deckPos = GetDeckPosition ();
			newCard.transform.position = deckPos;
			newCard.SetFlyTarget (deckPos, new Vector3 (x, y, z), FlyTime);
		}
	}

	void dealLeft ()
	{
		CardDef cLeft = Deck.Pop ();
		if (cLeft != null) {
			Debug.Log ("Deck-Popped for left");
			GameObject newObj = new GameObject ();
			newObj.name = "Card";
			Card newCard = newObj.AddComponent (typeof(Card)) as Card;
			newCard.Definition = cLeft;
			newObj.transform.parent = Deck.transform;
			newCard.TryBuild ();

			float x;
			float y;
			float z;
			
			Quaternion rot = transform.rotation;	
			rot = Quaternion.Euler (0, 0, 90);

			if (m_left.Count < 4) 
			{
				x = -7;
				y = -2 + (m_left.Count) * 1.5f;
				z = (m_left.Count) * -0.1f;

				newCard.transform.rotation = rot;

			} 
			else if (m_left.Count < 8) 
			{
				x = -8;
				y = -2 + (m_left.Count%4) * 1.5f;
				z = (m_left.Count) * -0.1f;	

				newCard.transform.rotation = rot;
			} 
			else 
			{
				x = -9;
				y = -2.75f + (m_left.Count%8) * 1.5f;
				z = (m_left.Count) * -0.1f;

				newCard.transform.rotation = rot;
			}

			m_left.Add (newCard);
			Vector3 deckPos = GetDeckPosition ();
			newCard.transform.position = deckPos;
			newCard.SetFlyTarget (deckPos, new Vector3 (x, y, z), FlyTime);
		}
	}
	
	void dealPlayer ()
	{
		CardDef cPlayer = Deck.Pop ();
		if (cPlayer != null) {
			Debug.Log ("Deck-Popped for player");
			GameObject newObj = new GameObject ();
			newObj.name = "Card";
			Card newCard = newObj.AddComponent (typeof(Card)) as Card;
			newCard.Definition = cPlayer;
			newObj.transform.parent = Deck.transform;
			newCard.TryBuild ();
			
			float x;
			float y;
			float z;

			if (m_player.Count < 4) 
			{
				x = -3 + (m_player.Count) * 1.5f;
				y = -3;
				z = (m_player.Count) * -0.1f;
			} 
			else if (m_player.Count < 8) 
			{
				x = -3 + (m_player.Count % 4) * 1.5f;
				y = -4;
				z = (m_player.Count) * -0.1f;	
			} 
			else {
				x = -3.75f + (m_player.Count % 8) * 1.5f;
				y = -5;
				z = (m_player.Count) * -0.1f;
			}
				
			m_player.Add (newCard);
			Vector3 deckPos = GetDeckPosition ();
			newCard.transform.position = deckPos;
			
			newCard.SetFlyTarget (deckPos, new Vector3 (x, y, z), FlyTime);
			
		}
	}

		
	
	
	static int Value(Card c)
	{
		if (c != null)
		{
			switch (c.Definition.Pattern)
			{
			case 0:
				return 10;
			case 1:
				return 11;
			}
			return c.Definition.Pattern;
		}
		return 0;
	}
	
	static int GetScore(List<Card> cards)
	{
		int score = 0;
		bool ace = false;
		foreach (Card c in cards)
		{
			int s = Value(c);
			if ((score + s) > 21)
			{
				if (s == 11)
				{
					s = 1;
				}
				else if (ace)
				{
					score -= 10;
					ace = false;
				}
			}
			score += s;
			ace |= (s == 11);
		}
		return score;
	}
	
	int GetDealerScore()
	{
		return GetScore(m_dealer);
	}
	
	int GetPlayerScore()
	{
		return GetScore(m_player);
	}
	
	const float DealTime = 0.2f;
	
	IEnumerator OnReset()
	{
		if (m_state != GameState.Resolving)
		{
			m_state = GameState.Resolving;
			ShowMessage("");
			clearAllHands ();
			
			Deck.Shuffle();
			
			for (int i = 0; i < 13; i++) {
				dealRight ();
				yield return new WaitForSeconds (DealTime);
				
				dealTop ();
				yield return new WaitForSeconds (DealTime);
				
				dealLeft ();
				yield return new WaitForSeconds (DealTime);
				
				dealPlayer ();
				yield return new WaitForSeconds (DealTime);
			}
			m_state = GameState.SelectingCards;

		}
	}
	
	
	bool TryFinalize ()
	{
		if (playerScore >= 101)
		{
			// Dealer Wins!
			ShowMessage("Player");
			m_state = GameState.PlayerWins;
			return true;
		}
		else if (rightScore >= 101)
		{
			ShowMessage("Dealer");
			m_state = GameState.DealerWins;
			return true;
		}
		else if (leftScore >= 101)
		{
			ShowMessage("Dealer");
			m_state = GameState.LeftWins;
			return true;
		}
		else if (topScore >= 101)
		{
			ShowMessage("Dealer");
			m_state = GameState.TopWins;
			return true;
		}
		/*
		// Natural 21 beats everything else.
		bool pn = (playerScore == 21) && (m_player.Count == 2);
		bool dn = (dealer == 21) && (m_dealer.Count == 2);
		if (pn && !dn)
		{
			ShowMessage("Player");
			m_state = GameState.PlayerWins;
			return true;
		}
		if (dn && !pn)
		{
			ShowMessage("Dealer");
			m_state = GameState.DealerWins;
			return true;
		}
		if (dealer > 17)
		{
			if (playerScore == dealer)
			{
				// Nobody Wins!
				ShowMessage("Nobody");
				m_state = GameState.NobodyWins;
				return true;
			}
			else if (dealer < playerScore)
			{
				// Player Wins!
				ShowMessage("Player");
				m_state = GameState.PlayerWins;
				return true;
			}
			else
			{
				// Dealer Wins!
				ShowMessage("Dealer");
				m_state = GameState.DealerWins;
				return true;
			}
		}*/
		return false;
	}
	
	IEnumerator readyToPass ()
	{
		if (m_state == GameState.SelectingCards && m_player_cards_to_pass.Count == 3)
		{
			m_state = GameState.PassingCards;
			
			Debug.Log ("Ready State!");
		}
		
		yield return new WaitForSeconds(DealTime);
	}
	
	public void OnButton(string msg)
	{
		Debug.Log("OnButton = "+msg);
		switch (msg)
		{
			case "Reset":
				StartCoroutine (OnReset ());
				break;
			case "Ready to Pass":
				StartCoroutine(readyToPass());
				break;
			case "Pass":
				StartCoroutine (playerPassCards ());
				break;
			case "Game Started":
				StartCoroutine (GamePlay());
				break;
		}
	}

	IEnumerator onPlayerSelectingCards (RaycastHit hit)
	{
		Card c = hit.collider.GetComponent<Card> ();

		Debug.Log (c.Definition.Text);
		
		if ( (m_player_cards_to_pass.Count != 3 || m_player_cards_to_pass.Contains (c)) && m_state == GameState.SelectingCards )
		{
			c.popUpCard ();
			
			
			
			if (!m_player_cards_to_pass.Contains (c))
			{
				Vector3 temp = new Vector3 (c.transform.position.x, c.transform.position.y - 0.5f, c.transform.position.z);
				CardLogistInfo cl = new CardLogistInfo (temp, c.transform.rotation);
				
				m_player_cards_to_pass.Add (c);
				m_player_cards_to_pass_positions.Add (cl);
				
				Debug.Log (m_player_cards_to_pass.Count + "   " + m_player_cards_to_pass_positions.Count);
			} else
			{
				Vector3 temp = new Vector3 (c.transform.position.x, c.transform.position.y, c.transform.position.z);
				
				m_player_cards_to_pass.Remove (c);
				m_player_cards_to_pass_positions.Remove (m_player_cards_to_pass_positions.Find ((x => x.position == temp)));
				
				Debug.Log (m_player_cards_to_pass.Count + "   " + m_player_cards_to_pass_positions.Count);
			}
		} 
		yield return new WaitForSeconds(DealTime);
	}

	
	
	bool tryPassCards ()
	{
		if (m_state == GameState.SelectingCards) //meaning that there is someone still selecting cards 
		{
			return true;
		}
		return false;
	}

	
	
	void botLeftSelectingPassingCards ()
	{
		for (int i = 0; i < 3; i++)
		{
			Card c = m_left [i];

			Vector3 temp = new Vector3 (c.transform.position.x, c.transform.position.y, c.transform.position.z);

			CardLogistInfo cl = new CardLogistInfo (temp, c.transform.rotation);

			m_left_cards_to_pass.Add (c);

			m_left_cards_to_pass_positions.Add (cl);
		}
		
		
		for (int i=0; i<3; i++) 
		{

			Card toPass = m_left_cards_to_pass [i];
			m_left.Remove (toPass);
			m_player.Add (toPass);


			Vector3 oldPos = toPass.transform.position;

			Vector3 newPos = m_player_cards_to_pass_positions [i].position;
			toPass.transform.rotation = m_player_cards_to_pass_positions [i].rot;


			toPass.SetFlyTarget (oldPos, newPos, FlyTime);	
		}
		
		
		m_state = GameState.PassingCards;	
	}
	
	
	
	void botTopSelectingPassingCards ()
	{
		for (int i = 0; i < 3; i++)
		{
			Card c = m_top [i];

			Vector3 temp = new Vector3 (c.transform.position.x, c.transform.position.y, c.transform.position.z); 

			CardLogistInfo cl = new CardLogistInfo (temp, c.transform.rotation);

			m_top_cards_to_pass.Add (c);

			m_top_cards_to_pass_positions.Add (cl);
		}

		for (int i=0; i<3; i++) 
		{

			Card toPass = m_top_cards_to_pass [i];
			m_top.Remove (toPass);
			m_left.Add (toPass);


			Vector3 oldPos = toPass.transform.position;

			Vector3 newPos = m_left_cards_to_pass_positions [i].position;
			toPass.transform.rotation = m_left_cards_to_pass_positions [i].rot;


			toPass.SetFlyTarget (oldPos, newPos, FlyTime);	
		}
		m_state = GameState.PassingCards;	
	}
	
	
	
	void botRightSelectingPassingCards ()
	{
		/*
		for (int i = 0; i < 3; i++)
		{

			Card c = m_right [i];

			Vector3 temp = new Vector3 (c.transform.position.x, c.transform.position.y, c.transform.position.z); 

			CardLogistInfo cl = new CardLogistInfo (temp, c.transform.rotation);

			m_right_cards_to_pass.Add (c);

			m_right_cards_to_pass_positions.Add (cl);

		}
		*/
		for (int i=0; i<3; i++) 
		{
			

			Card toPass = m_right_cards_to_pass [i];
			m_right.Remove (toPass);
			m_top.Add (toPass);
			
			Vector3 oldPos = toPass.transform.position;

			Vector3 newPos = m_top_cards_to_pass_positions [i].position;
			toPass.transform.rotation = m_top_cards_to_pass_positions [i].rot;


			/*Vector3 oldPos = r.transform.position;
			Vector3 newPos = m_top_cards_to_pass [i].transform.position;
			r.transform.rotation = m_top_cards_to_pass [i].transform.rotation;*/


		
			toPass.SetFlyTarget (oldPos, newPos, FlyTime);
		}
		m_state = GameState.PassingCards;	
	}
	
	
	
	IEnumerator playerPassCards()
	{
		if (m_state == GameState.PassingCards) 
		{
			//For Init. to avoid errors
			for (int i = 0; i < 3; i++)
			{
				Card c = m_right [i];

				Vector3 temp = new Vector3 (c.transform.position.x, c.transform.position.y, c.transform.position.z);
				

				CardLogistInfo cl = new CardLogistInfo (temp, c.transform.rotation);

				m_right_cards_to_pass.Add (c);

				m_right_cards_to_pass_positions.Add (cl);
			}
			
			for (int i=0; i<3; i++) 
			{
				Card toPass = m_player_cards_to_pass [i];
				m_player.Remove (toPass);
				m_right.Add (toPass);
				
				
				Vector3 oldPos = toPass.transform.position;
				Vector3 newPos = m_right_cards_to_pass_positions [i].position;
				
				toPass.transform.rotation = m_right_cards_to_pass_positions [i].rot;

				toPass.SetFlyTarget (oldPos, newPos, FlyTime);	
			}
			
			botLeftSelectingPassingCards ();
			yield return new WaitForSeconds(DealTime);
			botTopSelectingPassingCards ();
			yield return new WaitForSeconds(DealTime);
			botRightSelectingPassingCards ();
			yield return new WaitForSeconds(DealTime);
			
			Debug.Log ("Passing Completed!");
			
			m_state = GameState.Started;
			
		}
		yield return new WaitForSeconds(DealTime);
		
		
		
	}
	
	void clearAllHands ()
	{
		playerScore = 0;
		leftScore = 0;
		rightScore = 0;
		topScore = 0;
		
		foreach (Card c in m_top)
		{
			GameObject.DestroyImmediate(c.gameObject);
		}
		m_top.Clear();
		
		foreach (Card c in m_top_cards_to_pass)
		{
			GameObject.DestroyImmediate(c.gameObject);
		};
		m_top_cards_to_pass.Clear();
		
		m_top_cards_to_pass_positions.Clear();
		
		
		foreach (Card c in m_left)
		{
			GameObject.DestroyImmediate(c.gameObject);
		}
		m_left.Clear();
		
		foreach (Card c in m_left_cards_to_pass)
		{
			GameObject.DestroyImmediate(c.gameObject);
		}
		m_left_cards_to_pass.Clear();
		
		m_left_cards_to_pass_positions.Clear();

		
		foreach (Card c in m_right)
		{
			GameObject.DestroyImmediate(c.gameObject);
		}
		m_right.Clear();
		
		foreach (Card c in m_right_cards_to_pass)
		{
			GameObject.DestroyImmediate(c.gameObject);
		}
		m_right_cards_to_pass.Clear();
		
		m_right_cards_to_pass_positions.Clear();

		
		foreach (Card c in m_player)
		{
			GameObject.DestroyImmediate(c.gameObject);
		}
		m_player.Clear();
		
		foreach (Card c in m_player_cards_to_pass)
		{
			GameObject.DestroyImmediate(c.gameObject);
		}
		m_player_cards_to_pass.Clear();
		
		m_player_cards_to_pass_positions.Clear();	
		
		Deck.Reset ();
	}
	
	
	
	
	IEnumerator GamePlay()
	{
		
		if (m_state == GameState.Started)
		{
			Debug.Log ("Game Play Started!");
			
			
			
			while ( !TryFinalize() )
			{
				if (starting_side == Turn.player)
				{
					while (!Input.GetKeyDown (KeyCode.Mouse0) || (!playerPlayedCorrectly))
					{
						Debug.Log ("Player is Thinking...");
						yield return null;
					}
					yield return new WaitForSeconds (1);
					m_rightPlay ();
					yield return new WaitForSeconds (1);
					m_topPlay ();
					yield return new WaitForSeconds (1);
					m_leftPlay ();
					yield return new WaitForSeconds (1);
				} 
				else if (starting_side == Turn.right)
				{
					m_rightPlay ();
					yield return new WaitForSeconds (1);
					m_topPlay ();
					yield return new WaitForSeconds (1);
					m_leftPlay ();
					yield return new WaitForSeconds (1);
					while (!Input.GetKeyDown (KeyCode.Mouse0) || (!playerPlayedCorrectly))
					{
						Debug.Log ("Player is Thinking...");
						yield return null;
					}
					yield return new WaitForSeconds (1);
					
				}
				else if (starting_side == Turn.top)
				{
					m_topPlay ();
					yield return new WaitForSeconds (1);
					m_leftPlay ();
					yield return new WaitForSeconds (1);
					while (!Input.GetKeyDown (KeyCode.Mouse0) || (!playerPlayedCorrectly))
					{
						Debug.Log ("Player is Thinking...");
						yield return null;
					}
					yield return new WaitForSeconds (1);
					m_rightPlay ();
					yield return new WaitForSeconds (1);
				}
				else if (starting_side == Turn.left)
				{
					m_leftPlay ();
					yield return new WaitForSeconds (1);
					while (!Input.GetKeyDown (KeyCode.Mouse0) || (!playerPlayedCorrectly))
					{
						Debug.Log ("Player is Thinking...");
						yield return null;
					}
					yield return new WaitForSeconds (1);
					m_rightPlay ();
					yield return new WaitForSeconds (1);
					m_topPlay ();
					yield return new WaitForSeconds (1);
				}
				
			
				
				
				int playerCardPattern = cardsOnTable [0].Definition.Pattern; 
				int topCardPattern = cardsOnTable [2].Definition.Pattern;
				int leftCardPattern = cardsOnTable [3].Definition.Pattern;
				int rightCardPattern = cardsOnTable [1].Definition.Pattern;
				
				int max = Mathf.Max (playerCardPattern, Mathf.Max (topCardPattern, Mathf.Max (leftCardPattern, rightCardPattern)));
				
				yield return new WaitForSeconds (2);
				if (max == playerCardPattern)
				{
					starting_side = Turn.player;
					Debug.Log ("Cards should go for Player...");
					Vector3 playerGatherPosition = new Vector3 (-1f, -6f, -0.2f);
					for (int i = 0; i < 4; i++)
					{
						cardsOnTable [i].SetFlyTarget (cardsOnTable [i].transform.position, playerGatherPosition, FlyTime);
					}
				} 
				else if (max == rightCardPattern)
				{
					starting_side = Turn.right;
					Debug.Log ("Cards should go for Right...");
					Vector3 rightGatherPosition = new Vector3 (9f, 0f, -0.2f);
					for (int i = 0; i < 4; i++)
					{
						cardsOnTable [i].SetFlyTarget (cardsOnTable [i].transform.position, rightGatherPosition, FlyTime);
					}
				} 
				else if (max == topCardPattern)
				{
					starting_side = Turn.top;
					Debug.Log ("Cards should go for Top......");
					Vector3 topGatherPosition = new Vector3 (-1f, 7f, -0.2f);
					for (int i = 0; i < 4; i++)
					{
						cardsOnTable [i].SetFlyTarget (cardsOnTable [i].transform.position, topGatherPosition, FlyTime);
					}
				}
				else if (max == leftCardPattern)
				{
					starting_side = Turn.left;
					Debug.Log ("Cards should go for Left......");
					Vector3 leftGatherPosition = new Vector3 (-11f, 0f, -0.2f);
					for (int i = 0; i < 4; i++)
					{
						cardsOnTable [i].SetFlyTarget (cardsOnTable [i].transform.position, leftGatherPosition, FlyTime);
					}
				}
				
				
				playerPlayedCorrectly = false;
				
				yield return new WaitForSeconds (2);
			}
			
			yield return new WaitForSeconds (DealTime);
		}
	}
	
	

	void onPlayerPlaying (RaycastHit hit)
	{
		Vector3 playerPlayPosition = new Vector3 (-1f, -1.15f, -0.2f);
		Card c = hit.collider.GetComponent<Card> ();
		
		cardsOnTable [0] = c;
		m_player.Remove (c);

		c.SetFlyTarget (c.transform.position, playerPlayPosition, FlyTime);
		
		//playerPlayedCorrectly = false;
		Debug.Log (m_player.Count);
	}
	
	
	void m_rightPlay ()
	{
		Vector3 rightPlayPosition = new Vector3 (0.78f, 0f, -0.2f);
		//Vector3 topPlayPosition = new Vector3 (-1f, 1.15f, -0.2f);
		//Vector3 leftPlayPosition = new Vector3 (-2.75f, 0f, -0.2f);
		//Vector3 playerPlayPosition = new Vector3 (-1f, -1.15f, -0.2f);
		
		int rnd = Random.Range (0, m_right.Count);
		
		Card c = m_right[rnd];
		
		cardsOnTable [1] = c;
		m_right.Remove (c);
		
		c.SetFlyTarget (c.transform.position, rightPlayPosition, FlyTime);
		
		Debug.Log (m_right.Count);
	}
	
	void m_topPlay ()
	{	
		Vector3 topPlayPosition = new Vector3 (-1f, 1.15f, -0.2f);
		int rnd = Random.Range (0, m_top.Count);
		
		Card c = m_top[rnd];
		
		cardsOnTable [2] = c;
		m_top.Remove (c);

		c.SetFlyTarget (c.transform.position, topPlayPosition, FlyTime);

		Debug.Log (m_top.Count);
	}
	
	void m_leftPlay ()
	{	
		Vector3 leftPlayPosition = new Vector3 (-2.75f, 0f, -0.2f);
		int rnd = Random.Range (0, m_left.Count);
		
		Card c = m_left[rnd];

		cardsOnTable [3] = c;
		m_left.Remove (c);

		c.SetFlyTarget (c.transform.position, leftPlayPosition, FlyTime);

		Debug.Log (m_left.Count);
	}
}
