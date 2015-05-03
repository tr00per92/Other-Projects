import simplegui
import random
import math

# helper function to start and restart the game
def new_game():
    global num_range, secret_number, guesses_remaining
    
    secret_number = random.randrange(0, num_range)
    if (num_range == 100):
        guesses_remaining = 7
    else:
        guesses_remaining = 10
        
    print "New game with range from 0 to", num_range
    print "Remaining guesses:", guesses_remaining

def range100():
    global num_range
    num_range = 100
    new_game()

def range1000():  
    global num_range
    num_range = 1000
    new_game()
    
def input_guess(guess):
    # main game logic  here	
    global secret_number, guesses_remaining
    
    guess_num = int(guess)
    if (guess_num == secret_number):
        print "Correct! You win!"
        new_game()
    else:    
        guesses_remaining -= 1
        if (guess_num > secret_number):
            print "Lower!"            
        else:
            print "Higher!" 
        if (guesses_remaining > 0):
            print "Remaining guesses:", guesses_remaining
        else:
            print "You are out of guesses. You lose."
            new_game()
    
# create frame
frame = simplegui.create_frame("Guess the number", 300, 300)

# register event handlers for control elements and start frame
frame.add_button("Range: 0 - 100", range100, 180)
frame.add_button("Range: 0 - 1000", range1000, 180)
frame.add_input("Enter a guess", input_guess, 175)
frame.start()

# call new_game 
num_range = 100
new_game()
