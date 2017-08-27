import sys
from bfs_solver import BfsSolver
from dfs_solver import DfsSolver
from a_star_solver import AStarSolver
from id_a_star_solver import IdAStarSolver

def solve(method = 'ida', board = (1,2,5,3,4,0,6,7,8)):
    if (method == 'bfs'):
        BfsSolver().solve(board)
    elif (method == 'dfs'):
        DfsSolver().solve(board)
    elif (method == 'ast'):
        AStarSolver().solve(board)
    elif (method == 'ida'):
        IdAStarSolver().solve(board)

#solve(sys.argv[1], tuple(map(int, sys.argv[2].split(','))))
solve()
