--
-- PostgreSQL database cluster dump
--

-- Started on 2026-04-15 12:36:37

\restrict xKSXUxWEvmhUd1y4sJvAM6z0ViYr4zFfbjkDyBf1bGuuaizLJHG3Adj2zITKgUF

SET default_transaction_read_only = off;

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;

--
-- Roles
--

CREATE ROLE postgres;
ALTER ROLE postgres WITH SUPERUSER INHERIT CREATEROLE CREATEDB LOGIN REPLICATION BYPASSRLS;

--
-- User Configurations
--








\unrestrict xKSXUxWEvmhUd1y4sJvAM6z0ViYr4zFfbjkDyBf1bGuuaizLJHG3Adj2zITKgUF

--
-- Databases
--

--
-- Database "template1" dump
--

\connect template1

--
-- PostgreSQL database dump
--

\restrict e8JO5qqs7nEb5P5EKo2sH6hiTjr8EZx8j40eeLRXl5tbE6bl19wlM8NQAysTOBX

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

-- Started on 2026-04-15 12:36:37

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

-- Completed on 2026-04-15 12:36:37

--
-- PostgreSQL database dump complete
--

\unrestrict e8JO5qqs7nEb5P5EKo2sH6hiTjr8EZx8j40eeLRXl5tbE6bl19wlM8NQAysTOBX

--
-- Database "KeeperPRO" dump
--

--
-- PostgreSQL database dump
--

\restrict pxgxZOihVeY2jrj2jmOUR0h10JJQsXBzK2ThpLBHJZNMXjUXlip553elCgdMjpm

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

-- Started on 2026-04-15 12:36:37

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4987 (class 1262 OID 16597)
-- Name: KeeperPRO; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE "KeeperPRO" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Russian_Russia.1251';


ALTER DATABASE "KeeperPRO" OWNER TO postgres;

\unrestrict pxgxZOihVeY2jrj2jmOUR0h10JJQsXBzK2ThpLBHJZNMXjUXlip553elCgdMjpm
\connect "KeeperPRO"
\restrict pxgxZOihVeY2jrj2jmOUR0h10JJQsXBzK2ThpLBHJZNMXjUXlip553elCgdMjpm

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 232 (class 1259 OID 16723)
-- Name: attachments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.attachments (
    attachment_id integer NOT NULL,
    request_id integer NOT NULL,
    visitor_id integer,
    file_path character varying(500) NOT NULL,
    file_type character varying(10) NOT NULL,
    CONSTRAINT attachments_file_type_check CHECK (((file_type)::text = ANY ((ARRAY['pdf'::character varying, 'jpg'::character varying])::text[])))
);


ALTER TABLE public.attachments OWNER TO postgres;

--
-- TOC entry 231 (class 1259 OID 16722)
-- Name: attachments_attachment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.attachments_attachment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.attachments_attachment_id_seq OWNER TO postgres;

--
-- TOC entry 4988 (class 0 OID 0)
-- Dependencies: 231
-- Name: attachments_attachment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.attachments_attachment_id_seq OWNED BY public.attachments.attachment_id;


--
-- TOC entry 222 (class 1259 OID 16628)
-- Name: departments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.departments (
    department_id integer NOT NULL,
    name character varying(255) NOT NULL
);


ALTER TABLE public.departments OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16627)
-- Name: departments_department_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.departments_department_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.departments_department_id_seq OWNER TO postgres;

--
-- TOC entry 4989 (class 0 OID 0)
-- Dependencies: 221
-- Name: departments_department_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.departments_department_id_seq OWNED BY public.departments.department_id;


--
-- TOC entry 224 (class 1259 OID 16637)
-- Name: employees; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employees (
    employee_id integer NOT NULL,
    last_name character varying(100) NOT NULL,
    first_name character varying(100) NOT NULL,
    middle_name character varying(100),
    department_id integer NOT NULL
);


ALTER TABLE public.employees OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16636)
-- Name: employees_employee_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.employees_employee_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employees_employee_id_seq OWNER TO postgres;

--
-- TOC entry 4990 (class 0 OID 0)
-- Dependencies: 223
-- Name: employees_employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_employee_id_seq OWNED BY public.employees.employee_id;


--
-- TOC entry 220 (class 1259 OID 16613)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    email character varying(255) NOT NULL,
    password_hash character varying(255) NOT NULL,
    registration_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16612)
-- Name: users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_user_id_seq OWNER TO postgres;

--
-- TOC entry 4991 (class 0 OID 0)
-- Dependencies: 219
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 230 (class 1259 OID 16703)
-- Name: visitorrequestlink; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.visitorrequestlink (
    id integer NOT NULL,
    request_id integer NOT NULL,
    visitor_id integer NOT NULL
);


ALTER TABLE public.visitorrequestlink OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 16702)
-- Name: visitorrequestlink_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.visitorrequestlink_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.visitorrequestlink_id_seq OWNER TO postgres;

--
-- TOC entry 4992 (class 0 OID 0)
-- Dependencies: 229
-- Name: visitorrequestlink_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.visitorrequestlink_id_seq OWNED BY public.visitorrequestlink.id;


--
-- TOC entry 226 (class 1259 OID 16653)
-- Name: visitors; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.visitors (
    visitor_id integer NOT NULL,
    last_name character varying(100) NOT NULL,
    first_name character varying(100) NOT NULL,
    middle_name character varying(100),
    phone character varying(20),
    email character varying(255),
    organization character varying(255),
    birth_date date NOT NULL,
    passport_series character varying(4) NOT NULL,
    passport_number character varying(6) NOT NULL,
    photo_path character varying(500)
);


ALTER TABLE public.visitors OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 16652)
-- Name: visitors_visitor_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.visitors_visitor_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.visitors_visitor_id_seq OWNER TO postgres;

--
-- TOC entry 4993 (class 0 OID 0)
-- Dependencies: 225
-- Name: visitors_visitor_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.visitors_visitor_id_seq OWNED BY public.visitors.visitor_id;


--
-- TOC entry 228 (class 1259 OID 16668)
-- Name: visitrequests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.visitrequests (
    request_id integer NOT NULL,
    user_id integer NOT NULL,
    type character varying(20) NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    purpose text NOT NULL,
    department_id integer NOT NULL,
    employee_id integer NOT NULL,
    status character varying(50) DEFAULT 'проверка'::character varying,
    rejection_reason text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT visitrequests_type_check CHECK (((type)::text = ANY ((ARRAY['personal'::character varying, 'group'::character varying])::text[])))
);


ALTER TABLE public.visitrequests OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 16667)
-- Name: visitrequests_request_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.visitrequests_request_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.visitrequests_request_id_seq OWNER TO postgres;

--
-- TOC entry 4994 (class 0 OID 0)
-- Dependencies: 227
-- Name: visitrequests_request_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.visitrequests_request_id_seq OWNED BY public.visitrequests.request_id;


--
-- TOC entry 4794 (class 2604 OID 16726)
-- Name: attachments attachment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attachments ALTER COLUMN attachment_id SET DEFAULT nextval('public.attachments_attachment_id_seq'::regclass);


--
-- TOC entry 4787 (class 2604 OID 16631)
-- Name: departments department_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.departments ALTER COLUMN department_id SET DEFAULT nextval('public.departments_department_id_seq'::regclass);


--
-- TOC entry 4788 (class 2604 OID 16640)
-- Name: employees employee_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);


--
-- TOC entry 4785 (class 2604 OID 16616)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);


--
-- TOC entry 4793 (class 2604 OID 16706)
-- Name: visitorrequestlink id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitorrequestlink ALTER COLUMN id SET DEFAULT nextval('public.visitorrequestlink_id_seq'::regclass);


--
-- TOC entry 4789 (class 2604 OID 16656)
-- Name: visitors visitor_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitors ALTER COLUMN visitor_id SET DEFAULT nextval('public.visitors_visitor_id_seq'::regclass);


--
-- TOC entry 4790 (class 2604 OID 16671)
-- Name: visitrequests request_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitrequests ALTER COLUMN request_id SET DEFAULT nextval('public.visitrequests_request_id_seq'::regclass);


--
-- TOC entry 4981 (class 0 OID 16723)
-- Dependencies: 232
-- Data for Name: attachments; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.attachments (attachment_id, request_id, visitor_id, file_path, file_type) FROM stdin;
\.


--
-- TOC entry 4971 (class 0 OID 16628)
-- Dependencies: 222
-- Data for Name: departments; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.departments (department_id, name) FROM stdin;
1	Отдел информационной безопасности
2	Лаборатория SCADA-систем
3	Административный отдел
4	Отдел пропускного режима
\.


--
-- TOC entry 4973 (class 0 OID 16637)
-- Dependencies: 224
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employees (employee_id, last_name, first_name, middle_name, department_id) FROM stdin;
1	Иванов	Сергей	Петрович	1
2	Петрова	Анна	Сергеевна	2
3	Сидоров	Алексей	Владимирович	3
4	Кузнецова	Мария	Игоревна	1
\.


--
-- TOC entry 4969 (class 0 OID 16613)
-- Dependencies: 220
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, email, password_hash, registration_date) FROM stdin;
1	guest1@example.com	2c103f2c4ed1e59c0b4e2e01821770fa	2026-04-15 10:14:59.089202
2	guest2@example.com	e9751db8c918b432d573b0c50ed57342	2026-04-15 10:14:59.089202
3	visitor@example.com	37b3a5ac5d7d05291774037be7dd58ae	2026-04-15 10:14:59.089202
\.


--
-- TOC entry 4979 (class 0 OID 16703)
-- Dependencies: 230
-- Data for Name: visitorrequestlink; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.visitorrequestlink (id, request_id, visitor_id) FROM stdin;
5	2	1
6	2	2
7	2	3
8	2	4
\.


--
-- TOC entry 4975 (class 0 OID 16653)
-- Dependencies: 226
-- Data for Name: visitors; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.visitors (visitor_id, last_name, first_name, middle_name, phone, email, organization, birth_date, passport_series, passport_number, photo_path) FROM stdin;
1	Кузнецов	Дмитрий	Алексеевич	+7 (999) 123-45-67	dmitry@example.com	ООО "ТехноСервис"	1990-05-15	4512	345678	\N
2	Смирнова	Елена	Игоревна	+7 (916) 987-65-43	elena@example.ru	АО "Инфосистемы"	1988-11-22	6712	987654	\N
3	Васильев	Андрей	Николаевич	+7 (903) 111-22-33	andrey@example.org	\N	1995-03-10	3421	556677	\N
4	Морозова	Ольга	Владимировна	+7 (912) 555-66-77	olga@mail.com	ООО "ПромТех"	2000-07-19	1234	567890	\N
\.


--
-- TOC entry 4977 (class 0 OID 16668)
-- Dependencies: 228
-- Data for Name: visitrequests; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.visitrequests (request_id, user_id, type, start_date, end_date, purpose, department_id, employee_id, status, rejection_reason, created_at) FROM stdin;
1	1	personal	2026-04-16	2026-04-17	Технический аудит	1	1	одобрена	\N	2026-04-15 10:15:17.92091
2	2	group	2026-04-18	2026-04-20	Экскурсия для партнеров	2	2	проверка	\N	2026-04-15 10:15:17.92091
3	1	personal	2026-04-25	2026-04-27	Совещание по безопасности	3	3	не одобрена	Не предоставлен скан паспорта	2026-04-15 10:15:17.92091
4	3	personal	2026-04-17	2026-04-18	Встреча с руководством	1	4	проверка	\N	2026-04-15 10:15:17.92091
\.


--
-- TOC entry 4995 (class 0 OID 0)
-- Dependencies: 231
-- Name: attachments_attachment_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.attachments_attachment_id_seq', 1, false);


--
-- TOC entry 4996 (class 0 OID 0)
-- Dependencies: 221
-- Name: departments_department_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.departments_department_id_seq', 4, true);


--
-- TOC entry 4997 (class 0 OID 0)
-- Dependencies: 223
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 4, true);


--
-- TOC entry 4998 (class 0 OID 0)
-- Dependencies: 219
-- Name: users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_user_id_seq', 3, true);


--
-- TOC entry 4999 (class 0 OID 0)
-- Dependencies: 229
-- Name: visitorrequestlink_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.visitorrequestlink_id_seq', 8, true);


--
-- TOC entry 5000 (class 0 OID 0)
-- Dependencies: 225
-- Name: visitors_visitor_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.visitors_visitor_id_seq', 4, true);


--
-- TOC entry 5001 (class 0 OID 0)
-- Dependencies: 227
-- Name: visitrequests_request_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.visitrequests_request_id_seq', 4, true);


--
-- TOC entry 4812 (class 2606 OID 16735)
-- Name: attachments attachments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attachments
    ADD CONSTRAINT attachments_pkey PRIMARY KEY (attachment_id);


--
-- TOC entry 4802 (class 2606 OID 16635)
-- Name: departments departments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.departments
    ADD CONSTRAINT departments_pkey PRIMARY KEY (department_id);


--
-- TOC entry 4804 (class 2606 OID 16646)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 4798 (class 2606 OID 16626)
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- TOC entry 4800 (class 2606 OID 16624)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4810 (class 2606 OID 16711)
-- Name: visitorrequestlink visitorrequestlink_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitorrequestlink
    ADD CONSTRAINT visitorrequestlink_pkey PRIMARY KEY (id);


--
-- TOC entry 4806 (class 2606 OID 16666)
-- Name: visitors visitors_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitors
    ADD CONSTRAINT visitors_pkey PRIMARY KEY (visitor_id);


--
-- TOC entry 4808 (class 2606 OID 16686)
-- Name: visitrequests visitrequests_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitrequests
    ADD CONSTRAINT visitrequests_pkey PRIMARY KEY (request_id);


--
-- TOC entry 4819 (class 2606 OID 16736)
-- Name: attachments attachments_request_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attachments
    ADD CONSTRAINT attachments_request_id_fkey FOREIGN KEY (request_id) REFERENCES public.visitrequests(request_id);


--
-- TOC entry 4820 (class 2606 OID 16741)
-- Name: attachments attachments_visitor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attachments
    ADD CONSTRAINT attachments_visitor_id_fkey FOREIGN KEY (visitor_id) REFERENCES public.visitors(visitor_id);


--
-- TOC entry 4813 (class 2606 OID 16647)
-- Name: employees employees_department_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_department_id_fkey FOREIGN KEY (department_id) REFERENCES public.departments(department_id);


--
-- TOC entry 4817 (class 2606 OID 16712)
-- Name: visitorrequestlink visitorrequestlink_request_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitorrequestlink
    ADD CONSTRAINT visitorrequestlink_request_id_fkey FOREIGN KEY (request_id) REFERENCES public.visitrequests(request_id);


--
-- TOC entry 4818 (class 2606 OID 16717)
-- Name: visitorrequestlink visitorrequestlink_visitor_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitorrequestlink
    ADD CONSTRAINT visitorrequestlink_visitor_id_fkey FOREIGN KEY (visitor_id) REFERENCES public.visitors(visitor_id);


--
-- TOC entry 4814 (class 2606 OID 16692)
-- Name: visitrequests visitrequests_department_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitrequests
    ADD CONSTRAINT visitrequests_department_id_fkey FOREIGN KEY (department_id) REFERENCES public.departments(department_id);


--
-- TOC entry 4815 (class 2606 OID 16697)
-- Name: visitrequests visitrequests_employee_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitrequests
    ADD CONSTRAINT visitrequests_employee_id_fkey FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 4816 (class 2606 OID 16687)
-- Name: visitrequests visitrequests_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.visitrequests
    ADD CONSTRAINT visitrequests_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(user_id);


-- Completed on 2026-04-15 12:36:37

--
-- PostgreSQL database dump complete
--

\unrestrict pxgxZOihVeY2jrj2jmOUR0h10JJQsXBzK2ThpLBHJZNMXjUXlip553elCgdMjpm

-- Completed on 2026-04-15 12:36:37

--
-- PostgreSQL database cluster dump complete
--

