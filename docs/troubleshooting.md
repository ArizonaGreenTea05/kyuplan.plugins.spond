# Troubleshooting

First look at kyuplan's log: the plugin logs through kyuplan's logging under `Kyuplan.Plugins.Spond.SpondApiDataSource`. Logins are logged as information, problems as warnings. Passwords are never logged.

## Messages

| Message | Meaning | What to do |
|---|---|---|
| *Spond credentials are not configured. …* | No usable account: a password and an e-mail or phone number are both needed. | Fill in the settings form; remember to save. |
| *Spond rejected the login of Spond account N (…). Check the credentials; accounts that require two-factor authentication are not supported.* | Spond said no. | Check e-mail/phone and password by logging in at spond.com. Turn two-factor authentication off for that account. |
| *Spond returned no groups for this account.* / *… for any of the N accounts.* | The login worked but Spond delivered no groups (or a throttled answer). | Is the account a member/administrator of the groups? Wait some minutes if you made many requests; add a backup account. |
| *Could not reach Spond: …* | Network problem or Spond is down. | Check the server's internet access, DNS and proxy; try again later. |
| *Spond account N … failed* (warning) | One account failed and the next is tried. | Only a problem if every account does. |
| *… returned nothing, … delivered: it is asked last for the next 06:00:00* (warning) | Spond ignored one account; it moved to the end of the queue. | Usually Spond throttling. Raise `CacheDuration`. |

## Problems

### The Members page stays empty

1. Does the log say credentials are missing or rejected? See above.
2. Is the data source really Spond? **Settings** → *Data source*. With `Spond:UseDemoData=true` kyuplan uses demo data instead, whatever else is set.
3. Is the account in the groups, as administrator? Try the same login in the Spond app.
4. Wait for the first synchronisation (about a minute) and reload.

### Members are there, events or attendance are missing

- Statistics ask for a period; events outside it are not requested. Check the period in kyuplan.
- Changes in Spond arrive after at most `CacheDuration`. Use kyuplan's refresh action to drop the cache.
- Events whose month came back empty are retried after two minutes.
- A non-administrator account may not see other members' answers: use an administrator.

### Trainers are missing

An owner of an event counts as a trainer only if they are a **member of one of the loaded groups**. Add the trainer to the group in Spond. Right after a restart, events may be mapped before the groups were loaded; they are corrected after at most two minutes.

### A training lands on the wrong day

The club time zone is a kyuplan setting. If it is wrong, times shift. The plugin converts Spond's UTC timestamps into that zone, including daylight-saving changes.

### The same account keeps being skipped

Spond throttles accounts that send many requests. Raise `Spond:CacheDuration`, keep kyuplan's synchronisation interval moderate, and do not share the account with other tools that poll Spond.

### A member's age is missing

Spond has no valid date of birth for them (a missing or malformed value). Fix it in Spond.

### The plugin does not appear on the Plugins page

A new installation installs it on the first start. If the machine had no network then, it is retried at each start. Install it by hand with `ArizonaGreenTea05/kyuplan.plugins.spond`. A plugin change restarts the API; wait for it to come back.

## Known limits

- No two-factor authentication.
- Accounts are alternatives, not merged; they must see the same groups.
- The cache is in memory, per process.
- Spond.API reports every failed request as an empty list, so a real outage and "no events" can look alike for a few minutes.

## Reporting a bug

Open an [issue](https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/issues) with the plugin version (Plugins page), kyuplan version, the relevant log lines and what you expected. **Remove e-mail addresses, names and passwords.** Security problems: see [SECURITY.md](../SECURITY.md).
