import time, math
from BaseAI_3 import BaseAI

class PlayerAI(BaseAI):
    def getMove(self, grid):
        bestMove = performIterativeDeepening(grid)
        return bestMove


def performIterativeDeepening(grid):
    start = time.clock()
    depth = 3
    bestMove = None

    while True:
        newBest = maximize(grid, depth)
        if newBest[0] is None:
            break
        else:
            bestMove = newBest[0]

        depth += 1

        if time.clock() - start > 0.018:
            break

    #print('depth: %s' % depth)
    return bestMove

def minimize(grid, depth, alpha, beta):
    if depth == 0 or not grid.canMove():
        return None, getHeuristicEvaluation(grid)

    minMove = None
    minScore = beta

    for cell in grid.getAvailableCells():
        newGrid = grid.clone()
        newGrid.setCellValue(cell, 2)

        result = maximize(newGrid, depth - 1, alpha, beta)
        if result[1] < minScore:
            minScore = result[1]
            minMove = cell

        if minScore <= alpha:
            break

        if minScore < beta:
            beta = minScore

    return minMove, minScore

def maximize(grid, depth, alpha = -10000, beta = 10000):
    if depth == 0 or not grid.canMove():
        return None, getHeuristicEvaluation(grid)

    maxMove = None
    maxScore = alpha

    for move in grid.getAvailableMoves():
        newGrid = grid.clone()
        newGrid.move(move)

        result = minimize(newGrid, depth - 1, alpha, beta)
        if result[1] > maxScore:
            maxScore = result[1]
            maxMove = move

        if maxScore >= beta:
            break

        if maxScore > alpha:
            alpha = maxScore

    return maxMove, maxScore

def getHeuristicEvaluation(grid):
    emptyCells = len(grid.getAvailableCells())
    maxTile = grid.getMaxTile()

    emptyCellsResult = 0 if emptyCells == 0 else math.log(emptyCells) * 2.7
    maxTileResult = math.log(maxTile) / math.log(2)
    monotonicityResult = getMonotonicity(grid)

    return emptyCellsResult + maxTileResult + monotonicityResult

def getMonotonicity(grid):
    totals = [0, 0, 0, 0]

    for x in range(4):
        current = 0
        next = current + 1
        while next < 4:
            while next < 4 and grid.getCellValue((x, next)) == 0:
                next += 1

            if next >= 4:
                next -= 1

            currentContent = grid.getCellValue((x, current))
            currentValue = 0 if currentContent == 0 else math.log(currentContent) / math.log(2)

            nextContent = grid.getCellValue((x, next))
            nextValue = 0 if nextContent == 0 else math.log(nextContent) / math.log(2)

            if currentValue > nextValue:
                totals[0] += nextValue - currentValue
            elif nextValue > currentValue:
                totals[1] += currentValue - nextValue

            current = next
            next += 1

    for y in range(4):
        current = 0
        next = current + 1
        while next < 4:
            while next < 4 and grid.getCellValue((next, y)) == 0:
                next += 1

            if next >= 4:
                next -= 1

            currentContent = grid.getCellValue((current, y))
            currentValue = 0 if currentContent == 0 else math.log(currentContent) / math.log(2)

            nextContent = grid.getCellValue((next, y))
            nextValue = 0 if nextContent == 0 else math.log(nextContent) / math.log(2)

            if currentValue > nextValue:
                totals[2] += nextValue - currentValue
            elif nextValue > currentValue:
                totals[3] += currentValue - nextValue

            current = next
            next += 1

    return max(totals[0], totals[1]) + max(totals[2], totals[3])
