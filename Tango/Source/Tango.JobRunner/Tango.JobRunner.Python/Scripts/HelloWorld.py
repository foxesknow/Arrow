job.log.Info("Hello Python world")

with job.context.CreateCommand(job.settings.database) as command:
    command.CommandText = "select count(*) from Locations";
    rows = command.ExecuteScalar()

    job.log.Info('There are %s rows' % rows)