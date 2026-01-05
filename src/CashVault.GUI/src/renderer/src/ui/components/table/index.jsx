/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const TableElement = styled.table`
  border-collapse: separate;
  width: 100%;
  border-spacing: 0 0.25rem;
  table-layout: fixed;

  & th:last-child {
    width: 40%;
  }

  & td {
    background: #dae1d6;
    padding: ${(p) => p.cellPadding};
    border: 1px solid #dae1d6;

    font-family: Poppins;
    font-weight: 500;
    font-size: 1.125rem;
    line-height: 1.5rem;
    letter-spacing: -2%;
  }

  & tr td:first-of-type {
    border-radius: 10px 0 0 10px;
  }

  & tr td:last-child {
    border-radius: 0 10px 10px 0;
  }

  ${(p) => {
    if (p.zebra) {
      return `
            & tr:nth-of-type(even) td {
              background-color: white;
              border: 1px solid white;
            }

        `
    }
  }}
`

const Cell = styled.td`
  overflow-wrap: break-word; //NOTE: Temporary solution for long text
  ${(p) => {
    if (p.fullRadius) {
      return `
            border-radius: 10px !important;
        `
    }
  }}

  ${(p) => {
    if (p.color) {
      return `
            background-color: ${p.color} !important;
            border: 1px solid ${p.color} !important;
        `
    }
  }}
`

const HeaderCell = styled.th`
  color: #6a6a6a;
  font-weight: 600;
  font-size: 1rem;
  line-height: 1.125rem;
  letter-spacing: 4%;
  text-transform: uppercase;
  text-align: left;
  padding: 0.5rem 0.5rem 0.5rem 1.125rem;
  overflow: hidden;
  text-overflow: ellipsis;

  text-align: ${(p) => p.align ?? 'center'};
`

const Container = styled.div`
  width: 100%;
  overflow-y: auto;
`

const Card = styled.div`
  background: var(--bg-dark);
  border-radius: 12px;
  padding: 1rem;
  margin-bottom: 1rem;
  font-family: Poppins;
  font-size: 1rem;
  font-weight: 500;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem 0.5rem;
  position: relative;
`

const CardRow = styled.div`
  display: flex;
  flex-direction: column;
  align-items: flex-start;

  font-size: 0.95rem;
  color: #333;

  & .header {
    font-weight: 600;
    color: #6a6a6a;
    text-transform: uppercase;
  }

  & .content {
    max-width: fit-content;
    overflow: hidden !important;
  }

  & .info-icon {
    position: absolute;
    top: 50%;
    right: 1rem;
    transform: translateY(-50%);
  }
`

const Table = ({
  cellPadding = '1rem 1.125rem',
  columns = [],
  data = [],
  zebra = false,
  rowColor = [],
  breakPointWidth = 768
}) => {
  const _zebra = rowColor?.length > 0 ? false : zebra

  if (window.innerWidth < breakPointWidth) {
    return (
      <Container>
        {data && data?.length === 0 && <Card>No data</Card>}

        {data?.map((r, i) => {
          var color = null
          var selected = rowColor?.find((x) => x.index === i)

          if (selected) {
            color = selected.color
          }

          return (
            <Card key={i} style={{ backgroundColor: color || '#dae1d6' }}>
              {columns?.map((c) => (
                <CardRow key={c.id}>
                  <span className="header">{c.value}</span>
                  <span className="content">{c.render ? c.render(r) : r[c.accessor]}</span>
                </CardRow>
              ))}
            </Card>
          )
        })}
      </Container>
    )
  }

  return (
    <Container>
      <TableElement cellPadding={cellPadding} zebra={_zebra}>
        <thead>
          <tr>
            {columns?.map((c) => (
              <HeaderCell key={c.id} style={{ width: c.width }} align={c.textAlign || 'center'}>
                {c.value}
              </HeaderCell>
            ))}
          </tr>
        </thead>

        <tbody>
          {data && data?.length === 0 && (
            <tr key={0}>
              <Cell colSpan={columns?.length} fullRadius={true}>
                No data
              </Cell>
            </tr>
          )}

          {data?.map((r, i) => {
            var color = null
            var selected = rowColor?.find((x) => x.index === i)

            if (selected) {
              color = selected.color
            }

            return (
              <tr key={i}>
                {columns?.map((c) => (
                  <Cell key={c.id} color={color}>
                    {c.render && c.render(r)}
                    {!c.render && <>{r[c.accessor]}</>}
                  </Cell>
                ))}
              </tr>
            )
          })}
        </tbody>
      </TableElement>
    </Container>
  )
}

export default Table
