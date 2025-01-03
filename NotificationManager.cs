namespace Services
{
    class NotificationManager
    {
        private static Dictionary<int, NotifyIcon> notifications = new Dictionary<int, NotifyIcon>();

        public static void ShowNotification(int id, string title, string message)
        {
            if (notifications.ContainsKey(id))
            {
                notifications[id].Dispose();
                notifications.Remove(id);
            }

            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                BalloonTipTitle = title,
                BalloonTipText = message,
                Visible = true
            };

            notifications[id] = notifyIcon;

            notifyIcon.ShowBalloonTip(3000);

            Task.Delay(5000).ContinueWith(_ =>
            {
                notifyIcon.Dispose();
                notifications.Remove(id);
            });
        }
    }
}