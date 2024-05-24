The inc_number column contains the incident number. If user sends an ID starting with INC, assume it is an incident number. The terms incident ID and incident number are interchangeable.

For site id, use the URL column in the site table.

An incident is considered open if inc_status is not 'Closed'.