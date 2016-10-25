from os import getcwd,listdir
from os.path import isdir, join, isfile
import numpy, math, random
import pcgEnemyHelper

#chooses a movement from preexisting files
movement_path = join(getcwd(), "movement_patterns/")
movement_patterns = list()
for f in listdir(movement_path):
    full_path = join(movement_path, f)
    movement_patterns.append(full_path)
#1 hot

types_of_attack = ["beam", "circle", "crescent", "inverseCircle", "oval", "petal", "shuriken"]
#array format of enemies: [name, movemement pattern, 1-hots]

creation_history_path = "creationHistory.csv"
if not isfile(creation_history_path):
    csv_file = open(creation_history_path, 'w')
    csv_file.write("0")
    csv_file.close()

creation_history = 0#list()
csv_file = open(creation_history_path, 'r')
creation_history = int(csv_file.readline())
csv_file.close()
# for row in csv_file:
    # row = row.replace('\n', '')
    # creation_history.append(row.split((',')))





print "How many new enemies do we need to create?"
num_to_gen = int(raw_input())
new_enemeies = list()
for i in range(num_to_gen):
    cSharpEnemy = open("NLEnemy" + str(creation_history + 1) + ".cs", 'w')
    #sample the number attacks from a normal distribution
    num_attacks = 1 + int(abs(math.floor(numpy.random.normal(0, 0.3))))
    print "creating with " + str(num_attacks) + " attacks."
    #calculate how many possibilities there are, so if they all exist, stop
    # f = math.factorial
    # tot_num_enemies = len(movement_patterns) * f(len(types_of_attack)) / f(num_attacks) / f(len(types_of_attack) - num_attacks)
    # collisions = 0
    movement = random.choice(movement_patterns)

    list_attacks = list()
    for c in range(num_attacks):
        atk = random.choice(types_of_attack)
        count = list_attacks.count(atk)
        if count == 0:
            list_attacks.append(atk)
        else:
            list_attacks.append(atk + str(count))
    pcgEnemyHelper.write(creation_history + 1 + i, list_attacks, movement, cSharpEnemy)
    cSharpEnemy.close()
creation_history += num_to_gen
csv_file = open(creation_history_path, 'w')
csv_file.write(str(creation_history))
csv_file.close()
