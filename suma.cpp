#include <stdio.h>
#include <math.h>
#include <iostream>

int k, a;

void main() // Funcion principal
{
    //k = (3 + 5) * 8 - (10 - 4) / 2;
    a = 1;

    // Prueba del do while
    do
    {
        printf("    Hola, a = ", a);
        //printf("\n");
        a = a + 1;
    } while (a <= 3);

    // Prueba del if, scanf y printf
    /*
    printf("Hola, agrega un numero: ");
    scanf("%d", &a);

    if (a > 0)
    {
        printf("El numero es positivo");
    }
    else
    {
        printf("El numero es negativo");
    }
    */

    /*
    printf("Hola, agrega un numero: ");
    scanf("%d", &a);
    printf("El numero es: ", a);
    */

    /*
    for (i = 0; i < 3; i++)
    {
        printf("\nHola");
        // k = i;
    }
    */
}