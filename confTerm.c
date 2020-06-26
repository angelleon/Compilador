#include <stdio.h>
#include <termios.h> //termios, TCSANOW, ECHO, ICANON
#include <unistd.h>  //STDIN_FILENO

/*
 * Cambia la configuracion del terminal para que las pulsaciones del teclado
 * sean enviadas a los procesos inmediatamente (sin presionar enter)
 * Esto es necesario para implementar ReadKey con una sola tecla
 */

static struct termios oldt, newt;

int confTerm(unsigned char c)
{
    if (c)
    {
        /*tcgetattr gets the parameters of the current terminal
        STDIN_FILENO will tell tcgetattr that it should write the settings
        of stdin to oldt*/
        tcgetattr(STDIN_FILENO, &oldt);
        /*now the settings will be copied*/
        newt = oldt;

        /*ICANON normally takes care that one line at a time will be processed
        that means it will return if it sees a "\n" or an EOF or an EOL*/
        newt.c_lflag &= ~(ICANON);

        /*Those new settings will be set to STDIN
        TCSANOW tells tcsetattr to change attributes immediately. */
        tcsetattr(STDIN_FILENO, TCSANOW, &newt);
    }
    else
    {
        /*restore the old settings*/
        tcsetattr(STDIN_FILENO, TCSANOW, &oldt);
    }
}