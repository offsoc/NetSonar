﻿using Avalonia.Controls.ApplicationLifetimes;

namespace NetSonar.Avalonia.Services;

public class ClipboardService(IClassicDesktopStyleApplicationLifetime lifeTime)
{
    public void CopyToClipboard(string text) => lifeTime.MainWindow?.Clipboard?.SetTextAsync(text);
}