from state import State

class Node:
    def __init__(self, state, previousNode = None, fromDirection = None):
        self.state = state
        self.parent = previousNode
        self.fromDirection = fromDirection
        self.depth = 0 if previousNode == None else previousNode.depth + 1
        self.totalCost = self.depth + state.manhattanDistance

    def getSuccessors(self):
        successors = []
        hole = self.state.board.index(0)

        if (hole != 0 and hole != 1 and hole != 2):
            self.__swapAndStore(hole - 3, hole, 'Up', successors)

        if (hole != 6 and hole != 7 and hole != 8):
            self.__swapAndStore(hole + 3, hole, 'Down', successors)

        if (hole != 0 and hole != 3 and hole != 6):
            self.__swapAndStore(hole - 1, hole, 'Left', successors)

        if (hole != 2 and hole != 5 and hole != 8):
            self.__swapAndStore(hole + 1, hole, 'Right', successors)

        return successors

    def __swapAndStore(self, first, second, direction, successors):
        newBoard = list(self.state.board)
        temp = newBoard[first]
        newBoard[first] = self.state.board[second]
        newBoard[second] = temp
        successors.append(Node(State(tuple(newBoard)), self, direction))
