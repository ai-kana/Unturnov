#include "main.h"
#include "commands.h"
#include "buttons.h"
#include "modals.h"
#include "tickets.h"

#include <concord/discord_codecs.h>
#include <stdlib.h>
#include <string.h>
#include <pthread.h>
#include <stdio.h>

void* console_thread_start(void* client) {
    char line[64];
    while (true) {
        scanf("%63[^\n]", line);
        if (strcmp(line, "shutdown") == 0) {
            discord_shutdown(client);
            exit(0);
        }
    }

    return NULL;
}

void on_channel_create(struct discord *client, const struct discord_channel *channel) {
    const size_t count = sizeof("ticket");

    if (strncmp(channel->name, "ticket", count) == 0) {
    }
}

void on_ready(struct discord *client, const struct discord_ready *event) {
    struct discord_create_guild_application_command params = {
        .name = "createticket",
        .description = "testing",
        .type = 1,
        .default_permission = true
    };
    
    discord_create_guild_application_command(client, event->application->id, GUILD_ID, &params, NULL);
}

void on_interaction(struct discord *client, const struct discord_interaction *event) {
    switch (event->type) {
        case DISCORD_INTERACTION_APPLICATION_COMMAND:
            handle_command(client, event);
            return;
        case DISCORD_INTERACTION_MESSAGE_COMPONENT:
            handle_button(client, event);
            return;
        case DISCORD_INTERACTION_MODAL_SUBMIT:
            handle_modal(client, event);
            return;
        default: 
            return;
    }
}

int main() {
    //const char* token = getenv("unturnov_token");
    //struct discord* client = discord_init(token);

    struct discord* client = discord_config_init("config.json");
    discord_add_intents(client, DISCORD_GATEWAY_MESSAGE_CONTENT);

    pthread_t console_thread;
    pthread_create(&console_thread, NULL, &console_thread_start, client);

    discord_set_on_message_create(client, &archive_message);
    discord_set_on_ready(client, &on_ready);
    discord_set_on_interaction_create(client, &on_interaction);
    discord_set_on_channel_create(client, &on_channel_create);

    discord_run(client);
    discord_cleanup(client);
    ccord_global_cleanup();
}
