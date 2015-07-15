﻿namespace Orchard.ContentManagement.Aspects {
    public interface ILocalizableAspect : IContent {
        string Culture { get ; }
        IContent MasterContentItem { get; }
    }
}
