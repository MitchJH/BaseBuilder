using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class EntityCollider
    {
        List<Entity> all_game_entities;

        public EntityCollider()
        {
            all_game_entities = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            all_game_entities.Add(entity);
        }
       
        public void Collide(GameTime gameTime)
        {
            Entity collidee;
            Entity collider;

            for (int i = 0; i < all_game_entities.Count; i++)
            {
                collider = all_game_entities[i];

                for (int j = 0; j < all_game_entities.Count; j++)
                {
                    collidee = all_game_entities[j];

                    if(collider != collidee)
                    {
                        Vector2 collidee_center = new Vector2(collidee.Position.X + collidee.Width, collidee.Position.Y + collidee.Height);
                        Vector2 collider_center = new Vector2(collider.Position.X + collider.Width, collider.Position.Y + collider.Height);

                        float r = collider.Radius + collidee.Radius;
                        Vector2 offset = collidee_center - collider_center;
                        float lensqr = offset.LengthSquared();

                        if (lensqr < r * r)
                        {                            
                            collider.Collide(collidee);
                            collidee.Collide(collider);
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            Collide(gameTime);
        }

    }
}
