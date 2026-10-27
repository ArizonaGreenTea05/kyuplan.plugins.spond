# How it works

kyuplan asks every data source the same few questions (`IDataSource`): *which groups are there* and *which events does a group have between two dates*. This plugin answers them from Spond using the [Spond.API](https://www.nuget.org/packages/Spond.API) package.

## Groups and members

For each Spond group the plugin delivers:

| kyuplan | From Spond |
|---|---|
| Group id, name | the group's id and name (the id stands in for a missing name) |
| Subgroups | the group's subgroups (id and name) |
| Member id | the member's id in the group |
| Member name | first and last name, joined and trimmed |
| Member subgroups | the ids of the subgroups the member belongs to |
| Date of birth | the day Spond wrote |

**Every member of the group is kept**, whether or not they answer events and whether or not they administer the group.

**Date of birth.** Spond sends an ISO timestamp (`2012-05-03T00:00:00Z`) or a plain date. Only the first ten characters are read, as written: converting the timestamp to a time zone could move it to the day before and change an age by a day. Anything that is not a date means *no date of birth* (kyuplan then cannot apply age rules to that member).

## Events

For a group and a period kyuplan gets the events with:

- **Id, name, start, end.** The name is Spond's heading. Times come from Spond's raw ISO timestamps and are expressed in the **club time zone** (kyuplan's setting), not the server's. An end before the start is raised to the start.
- **Cancelled** flag.
- **Answers** by member: *accepted*, *declined*, *unanswered*, and *other* (waiting list and unconfirmed). If Spond lists a member under several answers, the clearest wins in that order: accepted, declined, unanswered, other.
- **Trainers.** The owners (hosts) of an event are its trainers. Spond names an owner by the id of their *user profile*, not by their member id in the group, so the plugin translates both kinds of id to the member id using the loaded groups. An owner who is **not a member** of the groups is not a trainer. If the groups are not known yet, the ids are kept unchanged and the result is cached briefly, then mapped again.

### Which events are asked for

Spond filters events by their **end** time while kyuplan's statistics work on the start date. So the plugin widens the period by one day on each side and asks **month by month** (in club time, so a month starts at local midnight, also when that midnight is skipped by a daylight-saving change). Events that appear in two months are merged by id. Cancelled events are delivered (flagged); hidden events and comments are not requested.

## Caching

| What | Kept for |
|---|---|
| Groups (with members) | `CacheDuration` (15 min) |
| Events of one group and month | `CacheDuration`; **2 minutes** if the month was empty or mapped without the member lookup |
| Member lookup by profile | as long as the groups |

Everything is dropped when kyuplan calls *invalidate* (the refresh action) and at every restart.

## Sessions and limits of the Spond API

Spond.API has traits the plugin works around:

- The client is **not thread-safe**: all calls to Spond are serialised, one at a time.
- Tokens are **never refreshed**: the plugin logs in again after `LoginLifetime`.
- A **failed request looks like an empty list**. So an empty group list is treated as a probably expired token (log in again, ask again); an empty month on a session older than five minutes is retried once after a fresh login; and if nothing helps and several accounts exist, the next account is asked.

## What the plugin does not do

- It never writes to Spond; it only reads.
- It does not merge accounts, support two-factor authentication or use an official Spond API (the plugin goes through the Spond.API package).
- It adds no pages, endpoints or permissions to kyuplan.
