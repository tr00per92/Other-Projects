import resource, time
from state import State
from node import Node

class IdAStarSolver:
    def solve(self, board):
        start_time = time.time()

        startNode = Node(State(board))
        stack = [startNode]
        stackBoards = set([board])
        visitedBoards = set()

        searchCount = 0
        maxSize = 1
        maxDepth = 0

        searchLimit = startNode.totalCost

        while stack:
            maxSize = max(maxSize, len(stack))

            currentNode = stack.pop()
            stackBoards.discard(currentNode.state.board)
            visitedBoards.add(currentNode.state.board)

            if not currentNode.state.isGoal():
                searchCount += 1

                successors = currentNode.getSuccessors()
                successors.reverse()
                for successor in successors:
                    if successor.totalCost <= searchLimit and successor.state.board not in visitedBoards and successor.state.board not in stackBoards:
                        stack.append(successor)
                        stackBoards.add(successor.state.board)
                        maxDepth = max(maxDepth, successor.depth)

                if not stack:
                    searchLimit += 1
                    visitedBoards.clear()
                    stack.append(startNode)
                    stackBoards.add(board)

            else:
                depth = currentNode.depth
                directions = [currentNode.fromDirection]

                while currentNode.parent != None:
                    currentNode = currentNode.parent
                    directions.append(currentNode.fromDirection)

                directions.reverse()

                memory = resource.getrusage(resource.RUSAGE_SELF).ru_maxrss / 1024
                totalTime = time.time() - start_time

                with open('output.txt', 'w') as file:
                    file.write('path_to_goal: %s\n' % directions[1:])
                    file.write('cost_of_path: %s\n' % depth)
                    file.write('nodes_expanded: %s\n' % searchCount)
                    file.write('fringe_size: %s\n' % len(stack))
                    file.write('max_fringe_size: %s\n' % maxSize)
                    file.write('search_depth: %s\n' % depth)
                    file.write('max_search_depth: %s\n' % maxDepth)
                    file.write('running_time: %.10f\n' % totalTime)
                    file.write('max_ram_usage: %.10f' % memory)

                print('path_to_goal: %s' % directions[1:])
                print('cost_of_path: %s' % depth)
                print('nodes_expanded: %s' % searchCount)
                print('fringe_size: %s' % len(stack))
                print('max_fringe_size: %s' % maxSize)
                print('search_depth: %s' % depth)
                print('max_search_depth: %s' % maxDepth)
                print('running_time: %.10f' % totalTime)
                print('max_ram_usage: %.10f' % memory)

                return
