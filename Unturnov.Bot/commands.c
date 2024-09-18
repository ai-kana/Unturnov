#include "commands.h"
#include "ticket_command.h"
#include <string.h>

void handle_command(struct discord *client, const struct discord_interaction *event) {
    if (strcmp(event->data->name, "createticket") == 0) {
        command_ticket(client, event);
    }
}
