# ============================================================================ #
#                                                                              #
#                                                               /   /   \      #
#   Made By IsCoffeeTho                                       /    |      \    #
#                                                            |     |       |   #
#   Makefile                                                 |      \      |   #
#                                                            |       |     |   #
#   Last Edited: 11:05PM 23/11/2023                           \      |    /    #
#                                                               \   /   /      #
#                                                                              #
# ============================================================================ #

all: 
	dotnet build

clean:
	rm -rf ./obj
	rm -rf ./bin

fclean: clean

re: fclean all

.PHONY: all clean fclean re