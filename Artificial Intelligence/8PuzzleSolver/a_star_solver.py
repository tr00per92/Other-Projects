import resource, time
from queue import PriorityQueue
from state import State
from node import Node

class AStarSolver:
    def solve(self, board):
        start_time = time.time()

        queue = PriorityQueue()
        queue.put((0, time.time(), Node(State(board))))
        queueBoards = set([board])
        visitedBoards = set()

        searchCount = 0
        maxSize = 1
        maxDepth = 0

        while not queue.empty():
            maxSize = max(maxSize, queue.qsize())

            currentNode = queue.get()[2]
            queueBoards.discard(currentNode.state.board)
            visitedBoards.add(currentNode.state.board)

            if not currentNode.state.isGoal():
                successors = currentNode.getSuccessors()

                for successor in successors:
                    if successor.state.board not in visitedBoards and successor.state.board not in queueBoards:
                        queue.put((successor.totalCost, time.time(), successor))
                        queueBoards.add(successor.state.board)
                        maxDepth = max(maxDepth, successor.depth)

                searchCount += 1

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
                    file.write('fringe_size: %s\n' % queue.qsize())
                    file.write('max_fringe_size: %s\n' % maxSize)
                    file.write('search_depth: %s\n' % depth)
                    file.write('max_search_depth: %s\n' % maxDepth)
                    file.write('running_time: %.10f\n' % totalTime)
                    file.write('max_ram_usage: %.10f' % memory)

                print('path_to_goal: %s' % directions[1:])
                print('cost_of_path: %s' % depth)
                print('nodes_expanded: %s' % searchCount)
                print('fringe_size: %s' % queue.qsize())
                print('max_fringe_size: %s' % maxSize)
                print('search_depth: %s' % depth)
                print('max_search_depth: %s' % maxDepth)
                print('running_time: %.10f' % totalTime)
                print('max_ram_usage: %.10f' % memory)

                return
