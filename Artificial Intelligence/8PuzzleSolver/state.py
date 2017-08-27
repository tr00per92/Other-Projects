class State:
    Goal = (0,1,2,3,4,5,6,7,8)

    def __init__(self, board):
        self.board = board
        self.manhattanDistance = self.__getManhattanDist(board)

    def isGoal(self):
        return self.board == State.Goal

    def __getManhattanDist(self, board):
        dist = 0
        for x in range(1, 9):
            dist += abs(board.index(x) - x)
        return dist
