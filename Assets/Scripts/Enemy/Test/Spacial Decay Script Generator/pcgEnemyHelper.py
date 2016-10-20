def tab(num):
    return "    " * num
#might be interesting to write a script that fills out a template with <variablename>'s, then
#wouldn't be hardcoding per type
def write(enemy_num, list_bullets, movement_pattern, file_writeto):
    file_writeto.write("using UnityEngine;\n")
    file_writeto.write("using System.Collections;\n")
    file_writeto.write("using DanmakU;\n")
    file_writeto.write("using DanmakU.Controllers;\n")
    file_writeto.write("\n")
    file_writeto.write("public class NLEnemy" + str(enemy_num) + ": Enemy \n{\n")
    file_writeto.write(tab(1) + "//movement type is " + movement_pattern + "\n")
    for bullet in list_bullets:
        file_writeto.write(tab(1) + "public DanmakuPrefab " + bullet +"Prefab;\n")
    file_writeto.write("\n")
    for bullet in list_bullets:
        file_writeto.write(tab(1) + "private FireBuilder " + bullet + "FireData; \n")
    # file_writeto.write("\n")

    file_writeto.write("\n")
    file_writeto.write(tab(1) + "public override void Start()\n" + tab(1) + "{\n")
    for bullet in list_bullets:
        prefab_var = bullet + "Prefab"
        fire_builder = bullet + "FireData"
        file_writeto.write(tab(2) + fire_builder + " = new FireBuilder(" + prefab_var + ", Field);\n")
        file_writeto.write(tab(2) + fire_builder + ".From(transform);\n")
        file_writeto.write(tab(2) + fire_builder + ".WithSpeed(6);\n")
        #TODO: select a modifier
        file_writeto.write(tab(2) + fire_builder + ".WithModifier();\n")
        file_writeto.write(tab(2) + fire_builder + ".WithController(new AccelerationController(3));\n")
        file_writeto.write("\n")
    file_writeto.write(tab(1) + "}\n")
    file_writeto.write("\n")

    writeMovement(movement_pattern, file_writeto)
    file_writeto.write("\n")

    #TODO: write attack method
    file_writeto.write(tab(1) + "private IEnumerator Attack()\n" + tab(1) + "{\n")
    file_writeto.write(tab(2) + "while (true)\n" + tab(2) + "{\n")
    for bullet in list_bullets:
        file_writeto.write(tab(3) + bullet + "FireData.Fire();\n")
        file_writeto.write(tab(3) + "yield return new WaitForSeconds(0.1f);\n")
    file_writeto.write(tab(3) + "yield return new WaitForSeconds(2.0f);\n")
    file_writeto.write(tab(2) + "}\n")
    file_writeto.write(tab(1) + "}\n")
    file_writeto.write("}")

#writes run()
# import os.isfile()
def writeMovement(movement_pattern, file_writeto):
    mf = open(movement_pattern, 'r')
    for line in mf:
        file_writeto.write(line)
