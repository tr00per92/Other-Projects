import simplegui

# define global variables
tenthSecond = 0
timerString = "0:00.0"
x = 0
y = 0

# define helper function format that converts time
def format(t):
    global timerString
    if t < 10:
        timerString = "0:00." + str(t)
    elif t >= 10 and t < 100:
        digit1 = t % 10
        digit2 = t / 10
        timerString = "0:0" + str(digit2) + "." + str(digit1)
    elif t >= 100 and t < 600:
        digit1 = t % 10
        digit2 = (t % 100) / 10
        digit3 = t / 100
        timerString = "0:" + str(digit3) + str(digit2) + "." + str(digit1)
    else:
        digit1 = t % 10
        digit2 = (t % 100) / 10
        digit3 = (t / 100) % 6
        digit4 = t / 600
        timerString = str(digit4) + ":" + str(digit3) + str(digit2) + "." + str(digit1)    
    
# define event handlers for buttons; "Start", "Stop", "Reset"
def start_handler():
    timer.start()

def stop_handler():
    global x, y
    
    if timer.is_running() and tenthSecond % 10 == 0:
        x += 1
        y += 1
    elif timer.is_running() and tenthSecond % 10 != 0:
        y += 1
        
    timer.stop()

def reset_handler():
    global timerString, tenthSecond, x, y
    timerString = "0:00.0"
    tenthSecond = 0
    x = 0
    y = 0
    timer.stop()

# define event handler for timer with 0.1 sec interval
def tick():
    global tenthSecond
    tenthSecond += 1
    format(tenthSecond)

# define draw handler
def draw(canvas):
    canvas.draw_text(timerString, [100, 180], 46, "White")
    canvas.draw_text(str(x), [230, 40], 28, "Red")
    canvas.draw_text("/", [250, 40], 28, "Red")
    canvas.draw_text(str(y), [270, 40], 28, "Red")
    
# create frame
frame = simplegui.create_frame("Timer", 300, 300)

# register event handlers
frame.add_button("Start", start_handler, 100)
frame.add_button("Stop", stop_handler, 100)
frame.add_button("Reset", reset_handler, 100)
frame.set_draw_handler(draw)
timer = simplegui.create_timer(100, tick)

# start frame
frame.start()
