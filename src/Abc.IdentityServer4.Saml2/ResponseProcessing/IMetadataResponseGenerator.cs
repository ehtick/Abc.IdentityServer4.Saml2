﻿// ----------------------------------------------------------------------------
// <copyright file="IMetadataResponseGenerator.cs" company="ABC software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//
//    Licensed under the Apache License, Version 2.0.
//    See LICENSE in the project root for license information.
// </copyright>
// ----------------------------------------------------------------------------

using Abc.IdentityModel.Metadata;
using System.Threading.Tasks;

namespace Abc.IdentityServer.Saml2.ResponseProcessing
{
    /// <summary>
    /// The SAML2 metadata response generator.
    /// </summary>
    public interface IMetadataResponseGenerator
    {
        Task<DescriptorBase> GenerateMetadata();
    }
}