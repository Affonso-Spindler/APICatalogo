using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APICatalogo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //via middleware
            //services.AddCors();

            //via atributo
            //permite apenas o uso do método GET
            services.AddCors(opt =>
            {
                opt.AddPolicy("PermitirApiRequest",
                    builder =>
                    builder.WithOrigins("http://www.apirequest.io").WithMethods("GET"));
            });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<ApiLoggingFilter>();
            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
                                            options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            /*JWT
             * adiciona o manipulador de autenticação e define o
             * esquema de autenticação usado : Bearer
             * Valida o emissor, a audiencia e a chave
             * usando a chave secreta Valida a assinatura
             */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(options =>
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidAudience = Configuration["TokenConfiguration:Audience"],
                        ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:key"]))

                    });


            //services.AddApiVersioning(options =>
            //{
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = new ApiVersion(1, 0);
            //    options.ReportApiVersions = true;

            //    //faz com que eu posso informar a versão no header da requisição, deixando a url mais limpa
            //    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            //});


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "APICatalogo",
                    Description = "Catálago de Produtos e Categorias",
                    TermsOfService = new Uri("https://affonso.net/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "affonso",
                        Email = "affonso_spindler@hotmai.com",
                        Url = new Uri("https://affonso.net"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Usar sobre LICX",
                        Url = new Uri("https://affonso.net/license"),
                    }
                });
                //a partir do nome do executável da minha aplicação, ele vai gerar um nome de arquivo xml
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { } },
                };

                c.AddSecurityDefinition(
                    "Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Copiar 'bearer' + token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    } });
            });


            services.AddControllers()//va ignorar a referencia cíclica na serialização da resposta Json no método action
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo v1"));
            }
            //Adiciona o middleWare de tratamento de Erros
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            //Add o middleware de roteamento
            app.UseRouting();

            //Add o middleware de autenticação
            app.UseAuthentication();

            //Add o middleware que habilita a autorização
            app.UseAuthorization();

            //Via middleware
            //permite que API receber requisições do caminho informado
            //app.UseCors(opt => opt.WithOrigins("https://www.apirequest.io"));

            //via Atributo
            app.UseCors();

            //Swagger
            app.UseSwagger();

            //SwaggerUI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "APICatalogo");
            });


            //Add o middleware que executa oo endpoint do Requenst Atual
            app.UseEndpoints(endpoints =>
            {
                //Add os endpoints para as Actions dos Controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}
