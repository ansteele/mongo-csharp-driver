/* Copyright 2010-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Core.Misc;
using MongoDB.Driver.Linq;

namespace MongoDB.Driver
{
    /// <summary>
    /// Base class for updates.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public abstract class UpdateDefinition<TDocument>
    {
        /// <summary>
        /// Renders the update to a <see cref="BsonValue"/>.
        /// </summary>
        /// <param name="documentSerializer">The document serializer.</param>
        /// <param name="serializerRegistry">The serializer registry.</param>
        /// <returns>A <see cref="BsonValue"/>.</returns>
        public virtual BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            return Render(documentSerializer, serializerRegistry, LinqProvider.V2);
        }

        /// <summary>
        /// Renders the update to a <see cref="BsonValue"/>.
        /// </summary>
        /// <param name="documentSerializer">The document serializer.</param>
        /// <param name="serializerRegistry">The serializer registry.</param>
        /// <param name="linqProvider">The LINQ provider.</param>
        /// <returns>A <see cref="BsonValue"/>.</returns>
        public abstract BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry, LinqProvider linqProvider);

        /// <summary>
        /// Performs an implicit conversion from <see cref="BsonDocument"/> to <see cref="UpdateDefinition{TDocument}"/>.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator UpdateDefinition<TDocument>(BsonDocument document)
        {
            if (document == null)
            {
                return null;
            }

            return new BsonDocumentUpdateDefinition<TDocument>(document);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="UpdateDefinition{TDocument}"/>.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator UpdateDefinition<TDocument>(string json)
        {
            if (json == null)
            {
                return null;
            }
            return new JsonUpdateDefinition<TDocument>(json);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="PipelineDefinition{TDocument, TDocument}"/> to <see cref="UpdateDefinition{TDocument}"/>.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator UpdateDefinition<TDocument>(PipelineDefinition<TDocument, TDocument> pipeline)
        {
            if (pipeline == null)
            {
                return null;
            }
            return new PipelineUpdateDefinition<TDocument>(pipeline);
        }
    }

    /// <summary>
    /// A <see cref="PipelineDefinition{TDocument,TDocument}"/> based update definition.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class PipelineUpdateDefinition<TDocument> : UpdateDefinition<TDocument>
    {
        private readonly PipelineDefinition<TDocument, TDocument> _pipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineUpdateDefinition{TDocument}"/> class.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public PipelineUpdateDefinition(PipelineDefinition<TDocument, TDocument> pipeline)
        {
            _pipeline = Ensure.IsNotNull(pipeline, nameof(pipeline));
        }

        /// <summary>
        /// Gets the pipeline.
        /// </summary>
        public PipelineDefinition<TDocument, TDocument> Pipeline => _pipeline;

        /// <summary>
        /// Renders the update to a <see cref="BsonValue"/> that represents <see cref="BsonArray"/>.
        /// </summary>
        /// <param name="documentSerializer">The document serializer.</param>
        /// <param name="serializerRegistry">The serializer registry.</param>
        /// <param name="linqProvider">The LINQ provider.</param>
        /// <returns>A <see cref="BsonValue"/>.</returns>
        public override BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry, LinqProvider linqProvider)
        {
            var renderedPipeline = _pipeline.Render(documentSerializer, serializerRegistry, linqProvider);
            return new BsonArray(renderedPipeline.Documents);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var inputSerializer = serializerRegistry.GetSerializer<TDocument>();
            return ToString(inputSerializer, serializerRegistry);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="inputSerializer">The input serializer.</param>
        /// <param name="serializerRegistry">The serializer registry.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(IBsonSerializer<TDocument> inputSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            return ToString(inputSerializer, serializerRegistry, LinqProvider.V2);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="inputSerializer">The input serializer.</param>
        /// <param name="serializerRegistry">The serializer registry.</param>
        /// <param name="linqProvider">The LINQ provider.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(IBsonSerializer<TDocument> inputSerializer, IBsonSerializerRegistry serializerRegistry, LinqProvider linqProvider)
        {
            var renderedPipeline = Render(inputSerializer, serializerRegistry, linqProvider);
            return renderedPipeline.ToJson();
        }
    }

         /// <summary>
    /// A <see cref="BsonDocument"/> based update.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class BsonDocumentUpdateDefinition<TDocument> : UpdateDefinition<TDocument>
    {
        private readonly BsonDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonDocumentUpdateDefinition{TDocument}"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public BsonDocumentUpdateDefinition(BsonDocument document)
        {
            _document = Ensure.IsNotNull(document, nameof(document));
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        public BsonDocument Document
        {
            get { return _document; }
        }

        /// <inheritdoc />
        public override BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry, LinqProvider linqProvider)
        {
            return _document;
        }
    }

    /// <summary>
    /// A JSON <see cref="String" /> based update.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class JsonUpdateDefinition<TDocument> : UpdateDefinition<TDocument>
    {
        private readonly string _json;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonUpdateDefinition{TDocument}"/> class.
        /// </summary>
        /// <param name="json">The json.</param>
        public JsonUpdateDefinition(string json)
        {
            _json = Ensure.IsNotNullOrEmpty(json, nameof(json));
        }

        /// <summary>
        /// Gets the json.
        /// </summary>
        public string Json
        {
            get { return _json; }
        }

        /// <inheritdoc />
        public override BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry, LinqProvider linqProvider)
        {
            return BsonDocument.Parse(_json);
        }
    }

    /// <summary>
    /// An <see cref="Object" /> based update.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class ObjectUpdateDefinition<TDocument> : UpdateDefinition<TDocument>
    {
        private readonly object _obj;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectUpdateDefinition{TDocument}"/> class.
        /// </summary>
        /// <param name="obj">The object.</param>
        public ObjectUpdateDefinition(object obj)
        {
            _obj = Ensure.IsNotNull(obj, nameof(obj));
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        public object Object
        {
            get { return _obj; }
        }

        /// <inheritdoc />
        public override BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry, LinqProvider linqProvider)
        {
            var serializer = serializerRegistry.GetSerializer(_obj.GetType());
            return new BsonDocumentWrapper(_obj, serializer);
        }
    }
}
