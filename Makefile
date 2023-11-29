# ============================================================================ #
#                                                                              #
#                                                               /   /   \      #
#   Made By IsCoffeeTho                                       /    |      \    #
#                                                            |     |       |   #
#   Makefile                                                 |      \      |   #
#                                                            |       |     |   #
#   Last Edited: 11:42PM 28/11/2023                           \      |    /    #
#                                                               \   /   /      #
#                                                                              #
# ============================================================================ #

MOD = BetterLCTerminal

all: dll package

dll:
	dotnet build

package:
	cp bin/Debug/netstandard2.1/${MOD}.dll mod/BepInEx/plugins/${MOD}.dll
	cd mod && zip -r ${MOD} ./*
	mv mod/${MOD}.zip ${MOD}.zip


clean:
	rm -rf ./obj
	rm -rf ./bin

fclean: clean

re: fclean all

.PHONY: all clean fclean re