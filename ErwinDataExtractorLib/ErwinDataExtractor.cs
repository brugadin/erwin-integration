using SCAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ErwinDataExtractorLib
{
    public class ErwinDataExtractor
    {
        public string ExtractErwinDataToJson(string erwinFilePath)
        {
            try
            {
                Application oApplication = (Application)Activator.CreateInstance(Type.GetTypeFromProgID("erwin9.SCAPI"));
                Console.WriteLine("ERwin Application initialized successfully.");

                var oPersistenceUnit = oApplication.PersistenceUnits.Add($"erwin://{erwinFilePath}", "RDO=Yes");
                var oSession = oApplication.Sessions.Add();
                oSession.Open(oPersistenceUnit);

                List<Entity> entities = ExtractModelProperties(oSession);

                string json = JsonConvert.SerializeObject(entities, Formatting.Indented);
                return json;
            }
            catch (Exception ex)
            {
                return $"Failed due to: {ex.Message}";
            }
        }

        private static List<Entity> ExtractModelProperties(Session session)
        {
            var entities = new List<Entity>();
            var relationships = new Dictionary<Guid, Relationship>();

            // Populate entities
            foreach (ModelObject mo in session.ModelObjects)
            {
                if (mo.ClassName == "Entity")
                {
                    var entity = new Entity
                    {
                        Name = mo.Name,
                        Id = ExtractGuidFromProperty(mo, "Long_Id")
                    };
                    entities.Add(entity);
                }
            }

            // Populate relationships and link them to entities
            foreach (ModelObject mo in session.ModelObjects)
            {
                if (mo.ClassName == "Relationship")
                {
                    var relationship = new Relationship
                    {
                        Name = mo.Name,
                        Id = ExtractGuidFromProperty(mo, "Long_Id"),
                        ParentId = ExtractGuidFromProperty(mo, "Parent_Entity_Ref"),
                        ChildId = ExtractGuidFromProperty(mo, "Child_Entity_Ref")
                    };

                    relationships[relationship.Id] = relationship;
                }
            }

            // Link relationships to entities
            foreach (var relationship in relationships.Values)
            {
                if (relationship.ParentId.HasValue)
                {
                    var parentEntity = entities.Find(e => e.Id == relationship.ParentId.Value);
                    if (parentEntity != null)
                    {
                        parentEntity.Relationships.Add(relationship);
                    }
                }

                if (relationship.ChildId.HasValue)
                {
                    var childEntity = entities.Find(e => e.Id == relationship.ChildId.Value);
                    if (childEntity != null)
                    {
                        childEntity.Relationships.Add(relationship);
                    }
                }
            }

            return entities;
        }

        private static Guid ExtractGuidFromProperty(ModelObject mo, string propertyName)
        {
            foreach (ModelProperty mp in mo.Properties)
            {
                if (mp.ClassName == propertyName)
                {
                    return TryParseGuidWithExtra((string)mp.Value);
                }
            }
            return Guid.Empty;
        }

        private static Guid TryParseGuidWithExtra(string input)
        {
            int index = input.IndexOf('+');
            if (index != -1)
            {
                string guidPart = input.Substring(0, index).Trim(new char[] { '{', '}' });
                if (Guid.TryParse(guidPart, out Guid result))
                {
                    return result;
                }
            }
            return Guid.Empty;
        }
    }

    public class Relationship
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ChildId { get; set; }
    }

    public class Entity
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public List<Relationship> Relationships { get; set; } = new List<Relationship>();
    }
}
