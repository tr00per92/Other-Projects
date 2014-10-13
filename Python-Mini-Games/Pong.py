import simplegui
import random

# global variables
WIDTH = 600
HEIGHT = 400       
BALL_RADIUS = 20
PAD_WIDTH = 8
PAD_HEIGHT = 80
HALF_PAD_WIDTH = PAD_WIDTH / 2
HALF_PAD_HEIGHT = PAD_HEIGHT / 2
LEFT = False
RIGHT = True

paddleLeft_pos = [HALF_PAD_WIDTH, HEIGHT/2 - HALF_PAD_HEIGHT]
paddleRight_pos = [WIDTH - HALF_PAD_WIDTH, HEIGHT/2 - HALF_PAD_HEIGHT]
paddleLeft_vel = 0
paddleRight_vel = 0

ball_pos = [WIDTH/2, HEIGHT/2]
ball_vel = [0, 0]

scoreLeft = 0
scoreRight = 0

# initialize ball_pos and ball_vel for the ball
def spawn_ball(direction):
    global ball_pos, ball_vel # these are vectors stored as lists
    
    ball_pos = [WIDTH/2, HEIGHT/2]
    
    if direction == RIGHT:
        ball_vel[0] = random.randrange(1, 4)
        ball_vel[1] = random.randrange(-4, -1)
    else:
        ball_vel[0] = random.randrange(-4, -1)
        ball_vel[1] = random.randrange(-4, -1)

# define event handlers
def new_game():
    global paddleLeft_pos, paddleRight_pos, paddleLeft_vel, paddleRight_vel  # these are numbers
    global scoreLeft, scoreRight  # these are ints
    
    paddleLeft_pos = [HALF_PAD_WIDTH, HEIGHT/2 - HALF_PAD_HEIGHT]
    paddleRight_pos = [WIDTH - HALF_PAD_WIDTH, HEIGHT/2 - HALF_PAD_HEIGHT]
    paddleLeft_vel = 0
    paddleRight_vel = 0
    
    scoreLeft = 0
    scoreRight = 0
    
    side = random.randrange(1, 3)
    if side == 1:
        spawn_ball(LEFT)
    else:
        spawn_ball(RIGHT)

def draw(canvas):
    global scoreLeft, scoreRight, paddleLeft_pos, paddleRight_pos, ball_pos, ball_vel
    
    # draw mid line and gutters
    canvas.draw_line([WIDTH / 2, 0],[WIDTH / 2, HEIGHT], 1, "White")
    canvas.draw_line([PAD_WIDTH, 0],[PAD_WIDTH, HEIGHT], 1, "White")
    canvas.draw_line([WIDTH - PAD_WIDTH, 0],[WIDTH - PAD_WIDTH, HEIGHT], 1, "White")
        
    # update ball
    ball_pos[0] += ball_vel[0]
    ball_pos[1] += ball_vel[1]
    
    if ball_pos[1] >= HEIGHT - BALL_RADIUS:
        ball_vel[1] = -ball_vel[1]
    elif ball_pos[1] <= BALL_RADIUS:
        ball_vel[1] = -ball_vel[1]
        
    if ball_pos[0] >= WIDTH - PAD_WIDTH - BALL_RADIUS:
        if ball_pos[1] >= paddleRight_pos[1] and ball_pos[1] <= paddleRight_pos[1] + PAD_HEIGHT:
            ball_vel[0] = -ball_vel[0]
        else:
            scoreLeft += 1
            spawn_ball(LEFT)
    elif ball_pos[0] <= PAD_WIDTH + BALL_RADIUS:
        if ball_pos[1] >= paddleLeft_pos[1] and ball_pos[1] <= paddleLeft_pos[1] + PAD_HEIGHT:
            ball_vel[0] = -ball_vel[0]
        else:
            scoreRight += 1
            spawn_ball(RIGHT)
    
    # draw ball
    canvas.draw_circle(ball_pos, BALL_RADIUS, 2, "White", "White")
    
    # update paddle's vertical position, keep paddle on the screen
    if paddleLeft_pos[1] + paddleLeft_vel >= 0 and paddleLeft_pos[1]+PAD_HEIGHT+ paddleLeft_vel <= HEIGHT:
        paddleLeft_pos[1] += paddleLeft_vel
    
    if paddleRight_pos[1]+ paddleRight_vel >= 0 and paddleRight_pos[1]+PAD_HEIGHT+ paddleRight_vel <= HEIGHT:
        paddleRight_pos[1] += paddleRight_vel
    
    # draw paddles
    canvas.draw_line([paddleLeft_pos[0], paddleLeft_pos[1]], [paddleLeft_pos[0], paddleLeft_pos[1] + PAD_HEIGHT], PAD_WIDTH, "White")
    canvas.draw_line([paddleRight_pos[0], paddleRight_pos[1]], [paddleRight_pos[0], paddleRight_pos[1] + PAD_HEIGHT], PAD_WIDTH, "White")
    
    # draw scores
    canvas.draw_text(str(scoreLeft), [WIDTH/2 - 50, 40], 30, "White")
    canvas.draw_text(str(scoreRight), [WIDTH/2 + 50, 40], 30, "White")   
        
def keydown(key):
    global paddleLeft_vel, paddleRight_vel
    
    if key == simplegui.KEY_MAP["W"]:
        paddleLeft_vel -= 5
    elif key == simplegui.KEY_MAP["S"]:
        paddleLeft_vel += 5
    elif key == simplegui.KEY_MAP["up"]:
        paddleRight_vel -= 5
    elif key == simplegui.KEY_MAP["down"]:
        paddleRight_vel += 5
   
def keyup(key):
    global paddleLeft_vel, paddleRight_vel
    
    if key == simplegui.KEY_MAP["W"]:
        paddleLeft_vel = 0
    elif key == simplegui.KEY_MAP["S"]:
        paddleLeft_vel = 0
    elif key == simplegui.KEY_MAP["up"]:
        paddleRight_vel = 0
    elif key == simplegui.KEY_MAP["down"]:
        paddleRight_vel = 0
        
def speed_up():
    global ball_vel
    
    if ball_vel[0] < 0:
        ball_vel[0] -= 1
    else:
        ball_vel[0] += 1
        
    if ball_vel[1] < 0:
        ball_vel[1] -= 1
    else:
        ball_vel[1] += 1

# create frame
frame = simplegui.create_frame("Pong", WIDTH, HEIGHT)
frame.set_draw_handler(draw)
frame.set_keydown_handler(keydown)
frame.set_keyup_handler(keyup)
frame.add_button('Restart', new_game, 100)

# create timer to speed up the ball during play
speed = simplegui.create_timer(2000, speed_up)

# start frame
new_game()
frame.start()
speed.start()
